using Microsoft.Extensions.DependencyInjection;
using Northwind.Shared.Abstractions;

namespace Northwind.Catalog;

[Module]
public sealed class CatalogModule : IModule
{
    public IServiceCollection AddModule(IServiceCollection services)
    {
        return services;
    }
}
