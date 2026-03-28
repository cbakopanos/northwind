using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Crm.Application;
using Northwind.Crm.Controllers;
using Northwind.Crm.Infrastructure;
using Northwind.Shared.Abstractions;

namespace Northwind.Crm;

[Module(order: 30)]
public sealed class CrmModule : IModule
{
    public IServiceCollection AddModule(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddCrmApplication()
            .AddCrmInfrastructure(configuration)
            .AddCrmControllers();

        return services;
    }

    public WebApplication MapEndpoints(WebApplication app)
    {
        app.MapCrmEndpoints();
        return app;
    }
}
