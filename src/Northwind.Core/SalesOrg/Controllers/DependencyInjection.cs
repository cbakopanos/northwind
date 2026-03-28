using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Northwind.SalesOrg.Controllers;

public static class DependencyInjection
{
    public static IServiceCollection AddSalesOrgControllers(this IServiceCollection services)
    {
        // TODO: Register SalesOrg controllers and API presentation services.
        return services;
    }

    public static WebApplication MapSalesOrgEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/sales-org");
        group.MapGet("/health", () => new { context = "SalesOrg", status = "ok" });
        return app;
    }
}
