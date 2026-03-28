using Microsoft.Extensions.DependencyInjection;
using Northwind.Shared.Abstractions;

namespace Northwind.SalesOrdering;

[Module]
public sealed class SalesOrderingModule : IModule
{
    public IServiceCollection AddModule(IServiceCollection services)
    {
        return services;
    }
}
