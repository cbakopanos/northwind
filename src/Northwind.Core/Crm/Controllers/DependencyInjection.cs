using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Northwind.Crm.Controllers;

public static class DependencyInjection
{
    public static IServiceCollection AddCrmControllers(this IServiceCollection services)
    {
        // TODO: Register Crm controllers and API presentation services.
        return services;
    }

    public static IEndpointRouteBuilder MapCrmEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/crm");
        group.MapGet("/health", () => new { context = "Crm", status = "ok" });
        return endpoints;
    }
}
