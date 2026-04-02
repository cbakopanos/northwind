using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Northwind.Shared.Abstractions;

namespace Northwind.SalesOrdering;

file static class Routes
{
    public const string Health = "/api/sales-ordering/health";
}

public sealed class SalesOrderingModule : IModule
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(Routes.Health, (ILogger<SalesOrderingModule> logger) =>
        {
            logger.LogInformation("Health endpoint requested for module {ModuleContext}", "SalesOrdering");
            return Results.Ok(new { context = "SalesOrdering", status = "ok", count = 0 });
        });

        return endpoints;
    }

    public void RegisterServices(IServiceCollection services, string connectionString)
    {
    }
}