using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Northwind.Shared.Abstractions;

namespace Northwind.Reporting;

[Module]
public sealed class ReportingModule : IModule
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/reporting/health", (ILogger<ReportingModule> logger) =>
        {
            logger.LogInformation("Health endpoint requested for module {ModuleContext}", "Reporting");
            return Results.Ok(new { context = "Reporting", status = "ok", count = 0 });
        });

        return endpoints;
    }
}
