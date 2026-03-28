using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Northwind.Reporting.Controllers;

public static class DependencyInjection
{
    public static IServiceCollection AddReportingControllers(this IServiceCollection services)
    {
        // TODO: Register Reporting controllers and API presentation services.
        return services;
    }

    public static WebApplication MapReportingEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/reporting");
        group.MapGet("/health", () => new { context = "Reporting", status = "ok" });
        return app;
    }
}
