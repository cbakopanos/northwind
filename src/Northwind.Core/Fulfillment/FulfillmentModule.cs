using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Fulfillment.Application;
using Northwind.Fulfillment.Infrastructure;
using Northwind.Fulfillment.Presentation;

namespace Northwind.Fulfillment;

public static class FulfillmentModule
{
    public static IServiceCollection AddFulfillmentModule(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddFulfillmentApplication()
            .AddFulfillmentInfrastructure(configuration)
            .AddFulfillmentPresentation();

        return services;
    }

    public static WebApplication MapFulfillmentModule(this WebApplication app)
    {
        app.MapFulfillmentEndpoints();
        return app;
    }
}
