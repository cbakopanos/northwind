using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Northwind.Shared.Abstractions;

namespace Northwind.Crm;

[Module]
public sealed class CrmModule : IModule
{
    public IServiceCollection AddModule(IServiceCollection services)
    {
        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/crm/health", (ILogger<CrmModule> logger) =>
        {
            logger.LogInformation("Health endpoint requested for module {ModuleContext}", "Crm");
            return Results.Ok(new { context = "Crm", status = "ok" });
        });

        return endpoints;
    }
}
