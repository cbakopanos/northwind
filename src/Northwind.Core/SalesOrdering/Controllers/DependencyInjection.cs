using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Northwind.SalesOrdering.Controllers;

public static class DependencyInjection
{
    public static IServiceCollection AddSalesOrderingControllers(this IServiceCollection services)
    {
        // TODO: Register SalesOrdering controllers and API presentation services.
        return services;
    }

    public static IEndpointRouteBuilder MapSalesOrderingEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/sales-ordering");
        group.MapGet("/health", () => new { context = "SalesOrdering", status = "ok" });
        return endpoints;
    }
}
