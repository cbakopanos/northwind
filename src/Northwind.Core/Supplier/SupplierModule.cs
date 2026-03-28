using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Supplier.Application;
using Northwind.Supplier.Controllers;
using Northwind.Supplier.Infrastructure;

namespace Northwind.Supplier;

public static class SupplierModule
{
    public static IServiceCollection AddSupplierModule(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSupplierApplication()
            .AddSupplierInfrastructure(configuration)
            .AddSupplierControllers();

        return services;
    }

    public static WebApplication MapSupplierModule(this WebApplication app)
    {
        app.MapSupplierEndpoints();
        return app;
    }
}
