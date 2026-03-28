using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Shared.Abstractions;

namespace Northwind;

public static class NorthwindCoreComposition
{
    private static readonly Lazy<IReadOnlyList<IModule>> Modules = new(DiscoverModules);

    public static IServiceCollection AddNorthwindCoreModules(this IServiceCollection services, IConfiguration configuration)
    {
        foreach (var module in Modules.Value)
        {
            services = module.AddModule(services, configuration);
        }

        return services;
    }

    public static WebApplication MapNorthwindCoreEndpoints(this WebApplication app)
    {
        IEndpointRouteBuilder endpoints = app;

        foreach (var module in Modules.Value)
        {
            endpoints = module.MapEndpoints(endpoints);
        }

        return app;
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
            .OrderBy(x => x.Attribute.Order)
            .ThenBy(x => x.Type.Name, StringComparer.Ordinal)
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
