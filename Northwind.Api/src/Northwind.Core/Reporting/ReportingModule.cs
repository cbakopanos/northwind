using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Northwind.Shared.Abstractions;

namespace Northwind.Reporting;

file static class Routes
{
    public const string Health = "/api/reporting/health";
}

public sealed class ReportingModule : IModule
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(Routes.Health, (ILogger<ReportingModule> logger) =>
        {
            logger.LogInformation("Health endpoint requested for module {ModuleContext}", "Reporting");
            return Results.Ok(new { context = "Reporting", status = "ok", count = 0 });
        });

        return endpoints;
    }

    public void RegisterServices(IServiceCollection services, string connectionString)
    {
    }
}