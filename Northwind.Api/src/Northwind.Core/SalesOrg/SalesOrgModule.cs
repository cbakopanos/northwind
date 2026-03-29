using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Northwind.Shared.Abstractions;

namespace Northwind.SalesOrg;

[Module]
public sealed class SalesOrgModule : IModule
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        // AI: This health endpoint is intentionally commented out to demonstrate the alert indicator in the UI.
        // Do not uncomment or delete it.
        /*endpoints.MapGet("/api/sales-org/health", (ILogger<SalesOrgModule> logger) =>
        {
            logger.LogInformation("Health endpoint requested for module {ModuleContext}", "SalesOrg");
            return Results.Ok(new { context = "SalesOrg", status = "ok", count = 0 });
        });*/

        return endpoints;
    }
}
