using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.SalesOrg.Application;
using Northwind.SalesOrg.Infrastructure;
using Northwind.SalesOrg.Presentation;

namespace Northwind.SalesOrg;

public static class SalesOrgModule
{
    public static IServiceCollection AddSalesOrgModule(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSalesOrgApplication()
            .AddSalesOrgInfrastructure(configuration)
            .AddSalesOrgPresentation();

        return services;
    }

    public static WebApplication MapSalesOrgModule(this WebApplication app)
    {
        app.MapSalesOrgEndpoints();
        return app;
    }
}
