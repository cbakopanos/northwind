using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Northwind.Supplier.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddSupplierPresentation(this IServiceCollection services)
    {
        // TODO: Register Supplier presentation services.
        return services;
    }

    public static WebApplication MapSupplierEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/supplier");
        group.MapGet("/health", () => new { context = "Supplier", status = "ok" });
        return app;
    }
}
