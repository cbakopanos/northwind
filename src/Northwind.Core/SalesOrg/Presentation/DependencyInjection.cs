using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Northwind.SalesOrg.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddSalesOrgPresentation(this IServiceCollection services)
    {
        // TODO: Register SalesOrg presentation services.
        return services;
    }

    public static WebApplication MapSalesOrgEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/sales-org");
        group.MapGet("/health", () => new { context = "SalesOrg", status = "ok" });
        return app;
    }
}
