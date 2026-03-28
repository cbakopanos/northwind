using Microsoft.Extensions.DependencyInjection;
using Northwind.Shared.Abstractions;

namespace Northwind.SalesOrg;

[Module]
public sealed class SalesOrgModule : IModule
{
    public IServiceCollection AddModule(IServiceCollection services)
    {
        return services;
    }
}
