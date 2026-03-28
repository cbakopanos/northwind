using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Northwind.Shared.Abstractions;

namespace Northwind.Fulfillment;

[Module]
public sealed class FulfillmentModule : IModule
{
    public IServiceCollection AddModule(IServiceCollection services)
    {
        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/fulfillment/health", (ILogger<FulfillmentModule> logger) =>
        {
            logger.LogInformation("Health endpoint requested for module {ModuleContext}", "Fulfillment");
            return Results.Ok(new { context = "Fulfillment", status = "ok" });
        });

        return endpoints;
    }
}
