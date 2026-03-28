using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Reporting.Application;
using Northwind.Reporting.Infrastructure;
using Northwind.Shared.Abstractions;

namespace Northwind.Reporting;

[Module(order: 70)]
public sealed class ReportingModule : IModule
{
    public IServiceCollection AddModule(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddReportingApplication()
            .AddReportingInfrastructure(configuration);

        return services;
    }
}
