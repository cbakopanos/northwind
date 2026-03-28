using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Northwind.Shared.Abstractions;
using Northwind.Supplier.Application;
using Northwind.Supplier.Infrastructure;

namespace Northwind.Supplier;

[Module]
[Infrastructure(typeof(SupplierDbContext))]
[Repository(typeof(ISuppliersRepository), typeof(SuppliersRepository))]
public sealed class SupplierModule : IModule
{
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

                var suppliers = await query.GetAllAsync(cancellationToken);

                logger.LogInformation("Returning {SupplierCount} suppliers", suppliers.Count);
                return Results.Ok(suppliers);
            })
            .WithName("GetAllSuppliers")
            .WithTags("Supplier")
            .Produces<IReadOnlyList<SupplierSummaryDto>>(StatusCodes.Status200OK);

        endpoints.MapGet(
            "/api/supplier/suppliers/{supplierId:int}",
            async (int supplierId, ISuppliersRepository query, ILogger<SupplierModule> logger, CancellationToken cancellationToken) =>
            {
                logger.LogInformation("Handling {Endpoint} for SupplierId {SupplierId}", "GET /api/supplier/suppliers/{supplierId:int}", supplierId);

                var supplier = await query.GetByIdAsync(supplierId, cancellationToken);

                if (supplier is null)
                {
                    logger.LogInformation("Supplier {SupplierId} not found", supplierId);
                    return Results.NotFound();
                }

                logger.LogInformation("Returning supplier {SupplierId}", supplierId);
                return Results.Ok(supplier);
            })
            .WithName("GetSupplierById")
            .WithTags("Supplier")
            .Produces<SupplierDetailsDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        return endpoints;
    }
}
