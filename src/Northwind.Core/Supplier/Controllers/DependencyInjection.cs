using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Shared.Extensions;

namespace Northwind.Supplier.Controllers;

public static class DependencyInjection
{
    public static IServiceCollection AddSupplierControllers(this IServiceCollection services)
    {
        return services.AddControllersLayer();
    }

    public static IEndpointRouteBuilder MapSupplierEndpoints(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapModuleHealthEndpoint("/api/supplier", "Supplier");
    }
}
