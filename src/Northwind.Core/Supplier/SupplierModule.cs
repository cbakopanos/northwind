using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Northwind.Shared.Abstractions;
using Northwind.Supplier.Application;
using Northwind.Supplier.Infrastructure;

namespace Northwind.Supplier;

[Module]
[Infrastructure(typeof(SupplierDbContext))]
public sealed class SupplierModule : IModule
{
    public IServiceCollection AddModule(IServiceCollection services)
    {
        services.AddScoped<ISuppliersRepository, SuppliersRepository>();

        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/supplier/health", (ILogger<SupplierModule> logger) =>
        {
            logger.LogInformation("Health endpoint requested for module {ModuleContext}", "Supplier");
            return Results.Ok(new { context = "Supplier", status = "ok" });
        });

        endpoints.MapGet(
            "/api/supplier/suppliers",
            async (ISuppliersRepository query, ILogger<SupplierModule> logger, CancellationToken cancellationToken) =>
            {
                logger.LogInformation("Handling {Endpoint}", "GET /api/supplier/suppliers");

                var suppliers = await query.Execute(cancellationToken);

                logger.LogInformation("Returning {SupplierCount} suppliers", suppliers.Count);
                return Results.Ok(suppliers);
            })
            .WithName("GetAllSuppliers")
            .WithTags("Supplier")
            .Produces<IReadOnlyList<SupplierListItem>>(StatusCodes.Status200OK);

        return endpoints;
    }
}
