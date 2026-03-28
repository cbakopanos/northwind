using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Northwind.SalesOrdering.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddSalesOrderingPresentation(this IServiceCollection services)
    {
        // TODO: Register SalesOrdering presentation services.
        return services;
    }

    public static WebApplication MapSalesOrderingEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/sales-ordering");
        group.MapGet("/health", () => new { context = "SalesOrdering", status = "ok" });
        return app;
    }
}
