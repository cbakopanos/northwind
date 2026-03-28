using Microsoft.Extensions.DependencyInjection;

namespace Northwind.Shared.Extensions;

public static class ModuleConventions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection AddControllersLayer(this IServiceCollection services)
    {
        return services;
    }
}
