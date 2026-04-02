using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Northwind.Shared.Abstractions;

namespace Northwind.SalesOrg;

file static class Routes
{
    public const string Health = "/api/sales-org/health";
}

public sealed class SalesOrgModule : IModule
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(Routes.Health, (ILogger<SalesOrgModule> logger) =>
        {
            logger.LogInformation("Health endpoint requested for module {ModuleContext}", "SalesOrg");
            return Results.Ok(new { context = "SalesOrg", status = "ok", count = 0 });
        });

        return endpoints;
    }

    public void RegisterServices(IServiceCollection services, string connectionString)
    {
    }
}