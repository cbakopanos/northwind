using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Northwind.Catalog.Controllers;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogControllers(this IServiceCollection services)
    {
        // TODO: Register Catalog controllers and API presentation services.
        return services;
    }

    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/catalog");
        group.MapGet("/health", () => new { context = "Catalog", status = "ok" });
        return endpoints;
    }
}
