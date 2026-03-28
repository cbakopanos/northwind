using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Shared.Abstractions;
using Northwind.Supplier.Application;
using Northwind.Supplier.Infrastructure;

namespace Northwind.Supplier;

[Module(order: 60)]
public sealed class SupplierModule : IModule
{
    public IServiceCollection AddModule(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSupplierApplication()
            .AddSupplierInfrastructure(configuration);

        return services;
    }
}
