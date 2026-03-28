using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Catalog.Application;
using Northwind.Catalog.Infrastructure;
using Northwind.Shared.Abstractions;

namespace Northwind.Catalog;

[Module(order: 20)]
public sealed class CatalogModule : IModule
{
    public IServiceCollection AddModule(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddCatalogApplication()
            .AddCatalogInfrastructure(configuration);

        return services;
    }
}
