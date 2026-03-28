using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Supplier.Application;
using Northwind.Supplier.Infrastructure;
using Northwind.Supplier.Presentation;

namespace Northwind.Supplier;

public static class SupplierModule
{
    public static IServiceCollection AddSupplierModule(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSupplierApplication()
            .AddSupplierInfrastructure(configuration)
            .AddSupplierPresentation();

        return services;
    }

    public static WebApplication MapSupplierModule(this WebApplication app)
    {
        app.MapSupplierEndpoints();
        return app;
    }
}
