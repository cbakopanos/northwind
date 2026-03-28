using Microsoft.Extensions.DependencyInjection;
using Northwind.Shared.Abstractions;

namespace Northwind.Reporting;

[Module]
public sealed class ReportingModule : IModule
{
    public IServiceCollection AddModule(IServiceCollection services)
    {
        return services;
    }
}
