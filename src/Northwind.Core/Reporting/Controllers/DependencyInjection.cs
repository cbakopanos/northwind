using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Shared.Extensions;

namespace Northwind.Reporting.Controllers;

public static class DependencyInjection
{
    public static IServiceCollection AddReportingControllers(this IServiceCollection services)
    {
        return services.AddControllersLayer();
    }

    public static IEndpointRouteBuilder MapReportingEndpoints(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapModuleHealthEndpoint("/api/reporting", "Reporting");
    }
}
