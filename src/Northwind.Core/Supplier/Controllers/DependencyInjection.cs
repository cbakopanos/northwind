using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Northwind.Supplier.Controllers;

public static class DependencyInjection
{
    public static IServiceCollection AddSupplierControllers(this IServiceCollection services)
    {
        // TODO: Register Supplier controllers and API presentation services.
        return services;
    }

    public static WebApplication MapSupplierEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/supplier");
        group.MapGet("/health", () => new { context = "Supplier", status = "ok" });
        return app;
    }
}
