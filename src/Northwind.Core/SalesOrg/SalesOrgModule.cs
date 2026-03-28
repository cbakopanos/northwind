using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Northwind.Shared.Abstractions;

namespace Northwind.SalesOrg;

[Module]
public sealed class SalesOrgModule : IModule
{
    public IServiceCollection AddModule(IServiceCollection services)
    {
        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/sales-org/health", (ILogger<SalesOrgModule> logger) =>
        {
            logger.LogInformation("Health endpoint requested for module {ModuleContext}", "SalesOrg");
            return Results.Ok(new { context = "SalesOrg", status = "ok" });
        });

        return endpoints;
    }
}
