using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Reporting.Application;
using Northwind.Reporting.Infrastructure;
using Northwind.Reporting.Presentation;

namespace Northwind.Reporting;

public static class ReportingModule
{
    public static IServiceCollection AddReportingModule(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddReportingApplication()
            .AddReportingInfrastructure(configuration)
            .AddReportingPresentation();

        return services;
    }

    public static WebApplication MapReportingModule(this WebApplication app)
    {
        app.MapReportingEndpoints();
        return app;
    }
}
