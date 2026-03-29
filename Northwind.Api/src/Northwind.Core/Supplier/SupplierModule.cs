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
        endpoints.MapGet("/api/supplier/health", async (ISuppliersRepository repository, ILogger<SupplierModule> logger, CancellationToken cancellationToken) =>
        {
            logger.LogInformation("Health endpoint requested for module {ModuleContext}", "Supplier");
            var count = await repository.GetCountAsync(cancellationToken);
            return Results.Ok(new { context = "Supplier", status = "ok", count = count });
        });

        endpoints.MapGet(
            "/api/supplier/suppliers",
            async (int? page, int? pageSize, ISuppliersRepository query, ILogger<SupplierModule> logger, CancellationToken cancellationToken) =>
            {
                var currentPage = page ?? 1;
                var currentPageSize = Math.Clamp(pageSize ?? 10, 1, 100);
                logger.LogInformation("Handling {Endpoint} (page {Page}, pageSize {PageSize})", "GET /api/supplier/suppliers", currentPage, currentPageSize);

                var result = await query.GetAllAsync(currentPage, currentPageSize, cancellationToken);

                logger.LogInformation("Returning {SupplierCount} of {TotalCount} suppliers", result.Items.Count, result.TotalCount);
                return Results.Ok(result);
            })
            .WithName("GetAllSuppliers")
            .WithTags("Supplier")
            .Produces<PagedResult<SupplierSummaryDto>>(StatusCodes.Status200OK);

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

        endpoints.MapPost(
            "/api/supplier/suppliers",
            async (SupplierRequest request, ISuppliersRepository repository, ILogger<SupplierModule> logger, CancellationToken cancellationToken) =>
            {
                logger.LogInformation("Handling {Endpoint}", "POST /api/supplier/suppliers");

                if (string.IsNullOrWhiteSpace(request.CompanyName))
                {
                    logger.LogInformation("Create supplier request rejected: CompanyName is required");
                    return Results.BadRequest(new { error = "CompanyName is required." });
                }

                var createdSupplierId = await repository.CreateAsync(request, cancellationToken);

                logger.LogInformation("Created supplier {SupplierId}", createdSupplierId);
                return Results.Created(
                    $"/api/supplier/suppliers/{createdSupplierId}",
                    new { supplierId = createdSupplierId });
            })
            .WithName("CreateSupplier")
            .WithTags("Supplier")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        endpoints.MapPut(
            "/api/supplier/suppliers/{supplierId:int}",
            async (int supplierId, SupplierRequest request, ISuppliersRepository repository, ILogger<SupplierModule> logger, CancellationToken cancellationToken) =>
            {
                logger.LogInformation("Handling {Endpoint} for SupplierId {SupplierId}", "PUT /api/supplier/suppliers/{supplierId:int}", supplierId);

                if (string.IsNullOrWhiteSpace(request.CompanyName))
                {
                    logger.LogInformation("Update supplier request rejected for SupplierId {SupplierId}: CompanyName is required", supplierId);
                    return Results.BadRequest(new { error = "CompanyName is required." });
                }

                var updated = await repository.UpdateAsync(supplierId, request, cancellationToken);

                if (!updated)
                {
                    logger.LogInformation("Supplier {SupplierId} not found for update", supplierId);
                    return Results.NotFound();
                }

                logger.LogInformation("Updated supplier {SupplierId}", supplierId);
                return Results.NoContent();
            })
            .WithName("UpdateSupplier")
            .WithTags("Supplier")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        return endpoints;
    }
}
