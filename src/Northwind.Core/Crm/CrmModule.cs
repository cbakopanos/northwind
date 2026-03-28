using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Crm.Application;
using Northwind.Crm.Infrastructure;
using Northwind.Crm.Presentation;

namespace Northwind.Crm;

public static class CrmModule
{
    public static IServiceCollection AddCrmModule(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddCrmApplication()
            .AddCrmInfrastructure(configuration)
            .AddCrmPresentation();

        return services;
    }

    public static WebApplication MapCrmModule(this WebApplication app)
    {
        app.MapCrmEndpoints();
        return app;
    }
}
