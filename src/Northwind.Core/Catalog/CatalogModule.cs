using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Northwind.Shared.Abstractions;

namespace Northwind.Catalog;

[Module]
public sealed class CatalogModule : IModule
{
    public IServiceCollection AddModule(IServiceCollection services)
    {
        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/catalog/health", (ILogger<CatalogModule> logger) =>
        {
            logger.LogInformation("Health endpoint requested for module {ModuleContext}", "Catalog");
            return Results.Ok(new { context = "Catalog", status = "ok" });
        });

        return endpoints;
    }
}
