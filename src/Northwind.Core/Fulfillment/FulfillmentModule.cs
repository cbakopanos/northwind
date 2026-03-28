using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Fulfillment.Application;
using Northwind.Fulfillment.Controllers;
using Northwind.Fulfillment.Infrastructure;

namespace Northwind.Fulfillment;

public static class FulfillmentModule
{
    public static IServiceCollection AddFulfillmentModule(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddFulfillmentApplication()
            .AddFulfillmentInfrastructure(configuration)
            .AddFulfillmentControllers();

        return services;
    }

    public static WebApplication MapFulfillmentModule(this WebApplication app)
    {
        app.MapFulfillmentEndpoints();
        return app;
    }
}
