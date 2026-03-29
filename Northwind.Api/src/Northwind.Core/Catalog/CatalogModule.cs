using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Northwind.Shared.Abstractions;

namespace Northwind.Catalog;

[Module]
public sealed class CatalogModule : IModule
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/catalog/health", (ILogger<CatalogModule> logger) =>
        {
            logger.LogInformation("Health endpoint requested for module {ModuleContext}", "Catalog");
            return Results.Ok(new { context = "Catalog", status = "ok", count = 0 });
        });

        return endpoints;
    }
}
