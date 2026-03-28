using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Shared.Abstractions;
using Northwind.Supplier.Application;
using Northwind.Supplier.Controllers;
using Northwind.Supplier.Infrastructure;

namespace Northwind.Supplier;

[Module(order: 60)]
public sealed class SupplierModule : IModule
{
    public IServiceCollection AddModule(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSupplierApplication()
            .AddSupplierInfrastructure(configuration)
            .AddSupplierControllers();

        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapSupplierEndpoints();
        return endpoints;
    }
}
