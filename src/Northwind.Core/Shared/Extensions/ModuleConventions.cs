using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

    public static IEndpointRouteBuilder MapModuleHealthEndpoint(
        this IEndpointRouteBuilder endpoints,
        string routePrefix,
        string context)
    {
        var group = endpoints.MapGroup(routePrefix);
        group.MapGet("/health", (ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("Northwind.ModuleHealth");
            logger.LogInformation("Health endpoint requested for module {ModuleContext}", context);
            return new { context, status = "ok" };
        });
        return endpoints;
    }
}
