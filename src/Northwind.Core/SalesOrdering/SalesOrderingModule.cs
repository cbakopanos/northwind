using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.SalesOrdering.Application;
using Northwind.SalesOrdering.Controllers;
using Northwind.SalesOrdering.Infrastructure;

namespace Northwind.SalesOrdering;

public static class SalesOrderingModule
{
    public static IServiceCollection AddSalesOrderingModule(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSalesOrderingApplication()
            .AddSalesOrderingInfrastructure(configuration)
            .AddSalesOrderingControllers();

        return services;
    }

    public static WebApplication MapSalesOrderingModule(this WebApplication app)
    {
        app.MapSalesOrderingEndpoints();
        return app;
    }
}
