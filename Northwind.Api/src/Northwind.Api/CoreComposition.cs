using Northwind.Shared.Abstractions;

namespace Northwind;

public static class CoreComposition
{
    private static readonly IReadOnlyList<IModule> Modules =
        typeof(IModule).Assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(IModule).IsAssignableFrom(t))
            .OrderBy(t => t.Name, StringComparer.Ordinal)
            .Select(t => (IModule)Activator.CreateInstance(t)!)
            .ToArray();

    public static IServiceCollection AddCoreModules(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Northwind")
                               ?? throw new InvalidOperationException(
                                   "Connection string 'Northwind' is not configured.");

        foreach (var module in Modules)
            module.RegisterServices(services, connectionString);

        return services;
    }

    

    public static IEndpointRouteBuilder MapCoreModuleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        foreach (var module in Modules)
            module.MapEndpoints(endpoints);
        
        return endpoints;
    }
}