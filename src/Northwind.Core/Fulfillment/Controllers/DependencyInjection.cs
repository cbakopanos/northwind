using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Northwind.Fulfillment.Controllers;

public static class DependencyInjection
{
    public static IServiceCollection AddFulfillmentControllers(this IServiceCollection services)
    {
        // TODO: Register Fulfillment controllers and API presentation services.
        return services;
    }

    public static IEndpointRouteBuilder MapFulfillmentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/fulfillment");
        group.MapGet("/health", () => new { context = "Fulfillment", status = "ok" });
        return endpoints;
    }
}
