using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Reporting.Application;
using Northwind.Reporting.Controllers;
using Northwind.Reporting.Infrastructure;

namespace Northwind.Reporting;

public static class ReportingModule
{
    public static IServiceCollection AddReportingModule(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddReportingApplication()
            .AddReportingInfrastructure(configuration)
            .AddReportingControllers();

        return services;
    }

    public static WebApplication MapReportingModule(this WebApplication app)
    {
        app.MapReportingEndpoints();
        return app;
    }
}
