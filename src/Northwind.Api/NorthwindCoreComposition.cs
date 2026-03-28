using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Shared.Abstractions;

namespace Northwind;

public static class NorthwindCoreComposition
{
    private static readonly Lazy<IReadOnlyList<IModule>> Modules = new(DiscoverModules);
    private const string DefaultNorthwindConnectionString = "Host=localhost;Port=5435;Database=northwind;Username=postgres;Password=postgres";

    public static IServiceCollection AddNorthwindCoreModules(this IServiceCollection services, IConfiguration configuration)
    {
        foreach (var module in Modules.Value)
        {
            services = services.AddModuleInfrastructure(module.GetType(), configuration);
            services = module.AddModule(services);
        }

        return services;
    }

    private static IServiceCollection AddModuleInfrastructure(
        this IServiceCollection services,
        Type moduleType,
        IConfiguration configuration)
    {
        var infrastructureAttributes = moduleType
            .GetCustomAttributes<InfrastructureAttribute>(inherit: false)
            .ToArray();

        foreach (var infrastructureAttribute in infrastructureAttributes)
        {
            if (!infrastructureAttribute.IsValidDbContextType)
            {
                throw new InvalidOperationException(
                    $"InfrastructureAttribute on '{moduleType.FullName}' has invalid DbContext type '{infrastructureAttribute.DbContextType.FullName}'.");
            }

            var connectionString = configuration.GetConnectionString(infrastructureAttribute.ConnectionStringName)
                ?? DefaultNorthwindConnectionString;

            services = services.AddDbContextByType(infrastructureAttribute.DbContextType, connectionString);
        }

        return services;
    }

    private static IServiceCollection AddDbContextByType(
        this IServiceCollection services,
        Type dbContextType,
        string connectionString)
    {
        var addDbContextMethod = typeof(EntityFrameworkServiceCollectionExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(method =>
                method.Name == nameof(EntityFrameworkServiceCollectionExtensions.AddDbContext)
                && method.IsGenericMethodDefinition
                && method.GetGenericArguments().Length == 1
                && method.GetParameters().Length == 4
                && method.GetParameters()[1].ParameterType == typeof(Action<DbContextOptionsBuilder>));

        var genericMethod = addDbContextMethod.MakeGenericMethod(dbContextType);

        genericMethod.Invoke(
            null,
            [
                services,
                (Action<DbContextOptionsBuilder>)(options => options.UseNpgsql(connectionString)),
                ServiceLifetime.Scoped,
                ServiceLifetime.Scoped
            ]);

        return services;
    }

    private static IReadOnlyList<IModule> DiscoverModules()
    {
        return typeof(IModule)
            .Assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IModule).IsAssignableFrom(t))
            .Select(t =>
            {
                var attribute = t.GetCustomAttribute<ModuleAttribute>(inherit: false);
                return attribute is null ? null : new ModuleDescriptor(t, attribute);
            })
            .Where(x => x is not null)
            .Select(x => x!)
            .OrderBy(x => x.Type.Name, StringComparer.Ordinal)
            .Select(x => CreateModuleInstance(x.Type))
            .ToArray();
    }

    private static IModule CreateModuleInstance(Type moduleType)
    {
        if (Activator.CreateInstance(moduleType) is IModule module)
        {
            return module;
        }

        throw new InvalidOperationException(
            $"Module '{moduleType.FullName}' could not be created. Ensure it has a public parameterless constructor.");
    }

    private sealed record ModuleDescriptor(Type Type, ModuleAttribute Attribute);
}
