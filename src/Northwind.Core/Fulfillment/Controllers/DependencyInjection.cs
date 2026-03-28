using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Northwind.Fulfillment.Controllers;

public static class DependencyInjection
{
    public static IServiceCollection AddFulfillmentControllers(this IServiceCollection services)
    {
        // TODO: Register Fulfillment controllers and API presentation services.
        return services;
    }

    public static WebApplication MapFulfillmentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/fulfillment");
        group.MapGet("/health", () => new { context = "Fulfillment", status = "ok" });
        return app;
    }
}
