using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Fulfillment.Application;
using Northwind.Fulfillment.Controllers;
using Northwind.Fulfillment.Infrastructure;
using Northwind.Shared.Abstractions;

namespace Northwind.Fulfillment;

[Module(order: 40)]
public sealed class FulfillmentModule : IModule
{
    public IServiceCollection AddModule(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddFulfillmentApplication()
            .AddFulfillmentInfrastructure(configuration)
            .AddFulfillmentControllers();

        return services;
    }

    public WebApplication MapEndpoints(WebApplication app)
    {
        app.MapFulfillmentEndpoints();
        return app;
    }
}
