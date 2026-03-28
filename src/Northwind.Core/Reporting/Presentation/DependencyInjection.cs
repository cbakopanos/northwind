using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Northwind.Reporting.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddReportingPresentation(this IServiceCollection services)
    {
        // TODO: Register Reporting presentation services.
        return services;
    }

    public static WebApplication MapReportingEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/reporting");
        group.MapGet("/health", () => new { context = "Reporting", status = "ok" });
        return app;
    }
}
