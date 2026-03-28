using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Catalog.Application;
using Northwind.Catalog.Infrastructure;
using Northwind.Catalog.Presentation;

namespace Northwind.Catalog;

public static class CatalogModule
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddCatalogApplication()
            .AddCatalogInfrastructure(configuration)
            .AddCatalogPresentation();

        return services;
    }

    public static WebApplication MapCatalogModule(this WebApplication app)
    {
        app.MapCatalogEndpoints();
        return app;
    }
}
