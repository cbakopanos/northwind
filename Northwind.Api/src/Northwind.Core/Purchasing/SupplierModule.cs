using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Northwind.Purchasing.Domain;
using Northwind.Shared.Abstractions;
using Northwind.Purchasing.Application;
using Northwind.Purchasing.Infrastructure;

namespace Northwind.Purchasing;

file static class Routes
{
    public const string Health      = "/api/purchasing/health";
    public const string Suppliers   = "/api/purchasing/suppliers";
    public const string SupplierById = "/api/purchasing/suppliers/{supplierId:int}";
}

public sealed class PurchasingModule : IModule
{
    public void RegisterServices(IServiceCollection services, string connectionString)
    {
        services.AddDbContext<SupplierDbContext>(o => o.UseNpgsql(connectionString));
        services.AddScoped<ISuppliersRepository, SuppliersRepository>();
        services.AddScoped<ISuppliersService, SuppliersService>();
    }
    
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(Routes.Health, async (ISuppliersService service, ILogger<PurchasingModule> logger,
            CancellationToken cancellationToken) =>
        {
            logger.LogInformation("Health endpoint requested for module {ModuleContext}", "Purchasing");
            var count = await service.GetCountAsync(cancellationToken);
            return Results.Ok(new { context = "Purchasing", status = "ok", count });
        })
            .WithName("GetPurchasingHealth")
            .WithTags("Purchasing")
            .AddEndpointFilter<HandlingLogFilter>()
            .Produces(StatusCodes.Status200OK);

        endpoints.MapGet(
                Routes.Suppliers,
                async (int? page, int? pageSize, ISuppliersService service, ILogger<PurchasingModule> logger,
                    CancellationToken cancellationToken) =>
                {
                    var currentPage = page ?? 1;
                    var currentPageSize = Math.Clamp(pageSize ?? 10, 1, 100);

                    var result = await service.GetAllAsync(currentPage, currentPageSize, cancellationToken);

                    logger.LogInformation("Returning {SupplierCount} of {TotalCount} suppliers", result.Items.Count,
                        result.TotalCount);
                    return Results.Ok(result);
                })
            .WithName("GetAllSuppliers")
            .WithTags("Purchasing")
            .AddEndpointFilter<HandlingLogFilter>()
            .AddEndpointFilter<PurchasingDomainValidationFilter>()
            .Produces<PagedResult<SupplierSummaryDto>>();

        endpoints.MapGet(
                Routes.SupplierById,
                async (int supplierId, ISuppliersService service, ILogger<PurchasingModule> logger,
                    CancellationToken cancellationToken) =>
                {
                    var supplier = await service.GetByIdAsync(supplierId, cancellationToken);

                    if (supplier is null)
                    {
                        logger.LogWarning("Supplier {SupplierId} not found", supplierId);
                        return Results.NotFound();
                    }

                    return Results.Ok(supplier);
                })
            .WithName("GetSupplierById")
            .WithTags("Purchasing")
            .AddEndpointFilter<HandlingLogFilter>()
            .AddEndpointFilter<PurchasingDomainValidationFilter>()
            .Produces<SupplierDetailsDto>()
            .Produces(StatusCodes.Status404NotFound);

        endpoints.MapPost(
                Routes.Suppliers,
                async (SupplierRequest request, ISuppliersService service,
                    CancellationToken cancellationToken) =>
                {
                    var createdSupplierId = await service.CreateAsync(request, cancellationToken);

                    return Results.Created(
                        $"/api/purchasing/suppliers/{createdSupplierId}",
                        new { supplierId = createdSupplierId });
                })
            .WithName("CreateSupplier")
            .WithTags("Purchasing")
            .AddEndpointFilter<HandlingLogFilter>()
            .AddEndpointFilter<PurchasingDomainValidationFilter>()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        endpoints.MapPut(
                Routes.SupplierById,
                async (int supplierId, SupplierRequest request, ISuppliersService service,
                    ILogger<PurchasingModule> logger, CancellationToken cancellationToken) =>
                {
                    var updated = await service.UpdateAsync(supplierId, request, cancellationToken);

                    if (!updated)
                    {
                        logger.LogWarning("Supplier {SupplierId} not found for update", supplierId);
                        return Results.NotFound();
                    }

                    return Results.NoContent();
                })
            .WithName("UpdateSupplier")
            .WithTags("Purchasing")
            .AddEndpointFilter<HandlingLogFilter>()
            .AddEndpointFilter<PurchasingDomainValidationFilter>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        return endpoints;
    }
}