using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Catalog;
using Northwind.Crm;
using Northwind.Fulfillment;
using Northwind.Reporting;
using Northwind.SalesOrdering;
using Northwind.SalesOrg;
using Northwind.Supplier;

namespace Northwind;

public static class NorthwindCoreComposition
{
    public static IServiceCollection AddNorthwindCoreModules(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSalesOrderingModule(configuration)
            .AddCatalogModule(configuration)
            .AddCrmModule(configuration)
            .AddFulfillmentModule(configuration)
            .AddSalesOrgModule(configuration)
            .AddSupplierModule(configuration)
            .AddReportingModule(configuration);

        return services;
    }

    public static WebApplication MapNorthwindCoreEndpoints(this WebApplication app)
    {
        app
            .MapSalesOrderingModule()
            .MapCatalogModule()
            .MapCrmModule()
            .MapFulfillmentModule()
            .MapSalesOrgModule()
            .MapSupplierModule()
            .MapReportingModule();

        return app;
    }
}
