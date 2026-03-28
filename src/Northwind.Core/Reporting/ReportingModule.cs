using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Reporting.Application;
using Northwind.Reporting.Controllers;
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
            .AddReportingInfrastructure(configuration)
            .AddReportingControllers();

        return services;
    }

    public WebApplication MapEndpoints(WebApplication app)
    {
        app.MapReportingEndpoints();
        return app;
    }
}
