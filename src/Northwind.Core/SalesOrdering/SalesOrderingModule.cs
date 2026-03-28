using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Northwind.Shared.Abstractions;

namespace Northwind.SalesOrdering;

[Module]
public sealed class SalesOrderingModule : IModule
{
    public IServiceCollection AddModule(IServiceCollection services)
    {
        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/sales-ordering/health", (ILogger<SalesOrderingModule> logger) =>
        {
            logger.LogInformation("Health endpoint requested for module {ModuleContext}", "SalesOrdering");
            return Results.Ok(new { context = "SalesOrdering", status = "ok" });
        });

        return endpoints;
    }
}
