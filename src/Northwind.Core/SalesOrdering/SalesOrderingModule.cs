using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.SalesOrdering.Application;
using Northwind.SalesOrdering.Infrastructure;
using Northwind.Shared.Abstractions;

namespace Northwind.SalesOrdering;

[Module(order: 10)]
public sealed class SalesOrderingModule : IModule
{
    public IServiceCollection AddModule(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSalesOrderingApplication()
            .AddSalesOrderingInfrastructure(configuration);

        return services;
    }
}
