using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Northwind.SalesOrg.Controllers;

public static class DependencyInjection
{
    public static IServiceCollection AddSalesOrgControllers(this IServiceCollection services)
    {
        // TODO: Register SalesOrg controllers and API presentation services.
        return services;
    }

    public static IEndpointRouteBuilder MapSalesOrgEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/sales-org");
        group.MapGet("/health", () => new { context = "SalesOrg", status = "ok" });
        return endpoints;
    }
}
