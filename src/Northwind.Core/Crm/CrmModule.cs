using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Shared.Abstractions;

namespace Northwind.Crm;

[Module]
public sealed class CrmModule : IModule
{
    public IServiceCollection AddModule(IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}
