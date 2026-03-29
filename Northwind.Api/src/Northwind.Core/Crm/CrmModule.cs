using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Northwind.Shared.Abstractions;

namespace Northwind.Crm;

[Module]
public sealed class CrmModule : IModule
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/crm/health", (ILogger<CrmModule> logger) =>
        {
            logger.LogInformation("Health endpoint requested for module {ModuleContext}", "Crm");
            return Results.Ok(new { context = "Crm", status = "ok", count = 0 });
        });

        return endpoints;
    }
}
