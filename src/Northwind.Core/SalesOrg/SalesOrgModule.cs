using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.SalesOrg.Application;
using Northwind.SalesOrg.Controllers;
using Northwind.SalesOrg.Infrastructure;
using Northwind.Shared.Abstractions;

namespace Northwind.SalesOrg;

[Module(order: 50)]
public sealed class SalesOrgModule : IModule
{
    public IServiceCollection AddModule(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSalesOrgApplication()
            .AddSalesOrgInfrastructure(configuration)
            .AddSalesOrgControllers();

        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapSalesOrgEndpoints();
        return endpoints;
    }
}
