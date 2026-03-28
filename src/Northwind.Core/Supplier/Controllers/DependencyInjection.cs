using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Northwind.Supplier.Controllers;

public static class DependencyInjection
{
    public static IServiceCollection AddSupplierControllers(this IServiceCollection services)
    {
        // TODO: Register Supplier controllers and API presentation services.
        return services;
    }

    public static IEndpointRouteBuilder MapSupplierEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/supplier");
        group.MapGet("/health", () => new { context = "Supplier", status = "ok" });
        return endpoints;
    }
}
