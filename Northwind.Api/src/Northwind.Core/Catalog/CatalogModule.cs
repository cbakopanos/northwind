using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Northwind.Catalog.Application;
using Northwind.Catalog.Infrastructure;
using Northwind.Shared.Abstractions;

namespace Northwind.Catalog;

file static class Routes
{
    public const string Health               = "/api/catalog/health";
    public const string Categories           = "/api/catalog/categories";
    public const string CategoryById         = "/api/catalog/categories/{categoryId:int}";
    public const string CategoryPicture      = "/api/catalog/categories/{categoryId:int}/picture";
    public const string Products             = "/api/catalog/products";
    public const string ProductById          = "/api/catalog/products/{productId:int}";
    public const string ProductDiscontinue   = "/api/catalog/products/{productId:int}/discontinue";
    public const string ProductReinstate     = "/api/catalog/products/{productId:int}/reinstate";
}

public sealed class CatalogModule : IModule
{
    public void RegisterServices(IServiceCollection services, string connectionString)
    {
        services.AddDbContext<CatalogDbContext>(o => o.UseNpgsql(connectionString));
        services.AddScoped<ICategoriesRepository, CategoriesRepository>();
        services.AddScoped<IProductsRepository, ProductsRepository>();
    }
    
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(Routes.Health, async (ICategoriesRepository categories, IProductsRepository products,
            ILogger<CatalogModule> logger, CancellationToken cancellationToken) =>
        {
            logger.LogInformation("Health endpoint requested for module {ModuleContext}", "Catalog");
            var categoryCount = await categories.GetCountAsync(cancellationToken);
            var productCount = await products.GetCountAsync(cancellationToken);
            return Results.Ok(new
                { context = "Catalog", status = "ok", categories = categoryCount, products = productCount });
        })
            .WithName("GetCatalogHealth")
            .WithTags("Catalog")
            .AddEndpointFilter<HandlingLogFilter>()
            .Produces(StatusCodes.Status200OK);

        // ── Categories ──────────────────────────────────────────────────────

        endpoints.MapGet(
                Routes.Categories,
                async (ICategoriesRepository query, CancellationToken cancellationToken) =>
                {
                    var categories = await query.GetAllAsync(cancellationToken);

                    return Results.Ok(categories);
                })
            .WithName("GetAllCategories")
            .WithTags("Catalog")
            .AddEndpointFilter<HandlingLogFilter>()
            .Produces<IReadOnlyList<CategorySummaryDto>>();

        endpoints.MapGet(
                Routes.CategoryById,
                async (int categoryId, ICategoriesRepository query, ILogger<CatalogModule> logger,
                    CancellationToken cancellationToken) =>
                {
                    var category = await query.GetByIdAsync(categoryId, cancellationToken);

                    if (category is null)
                    {
                        logger.LogWarning("Category {CategoryId} not found", categoryId);
                        return Results.NotFound();
                    }

                    return Results.Ok(category);
                })
            .WithName("GetCategoryById")
            .WithTags("Catalog")
            .AddEndpointFilter<HandlingLogFilter>()
            .Produces<CategoryDetailsDto>()
            .Produces(StatusCodes.Status404NotFound);

        endpoints.MapGet(
                Routes.CategoryPicture,
                async (int categoryId, ICategoriesRepository query, ILogger<CatalogModule> logger,
                    CancellationToken cancellationToken) =>
                {
                    var picture = await query.GetPictureAsync(categoryId, cancellationToken);

                    if (picture is null)
                    {
                        logger.LogWarning("Picture for category {CategoryId} not found", categoryId);
                        return Results.NotFound();
                    }

                    logger.LogInformation("Returning picture for category {CategoryId} ({PictureBytes} bytes)",
                        categoryId, picture.Length);
                    return Results.File(picture, "image/bmp");
                })
            .WithName("GetCategoryPicture")
            .WithTags("Catalog")
            .AddEndpointFilter<HandlingLogFilter>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        endpoints.MapPost(
                Routes.Categories,
                async (CategoryRequest request, ICategoriesRepository repository,
                    CancellationToken cancellationToken) =>
                {
                    var createdCategoryId = await repository.CreateAsync(request, cancellationToken);

                    return Results.Created(
                        $"/api/catalog/categories/{createdCategoryId}",
                        new { categoryId = createdCategoryId });
                })
            .WithName("CreateCategory")
            .WithTags("Catalog")
            .AddEndpointFilter<HandlingLogFilter>()
            .AddEndpointFilter<ValidationFilter<CategoryRequest>>()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        endpoints.MapPut(
                Routes.CategoryById,
                async (int categoryId, CategoryRequest request, ICategoriesRepository repository,
                    ILogger<CatalogModule> logger, CancellationToken cancellationToken) =>
                {
                    var updated = await repository.UpdateAsync(categoryId, request, cancellationToken);

                    if (!updated)
                    {
                        logger.LogWarning("Category {CategoryId} not found for update", categoryId);
                        return Results.NotFound();
                    }

                    return Results.NoContent();
                })
            .WithName("UpdateCategory")
            .WithTags("Catalog")
            .AddEndpointFilter<HandlingLogFilter>()
            .AddEndpointFilter<ValidationFilter<CategoryRequest>>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        // ── Products ────────────────────────────────────────────────────────

        endpoints.MapGet(
                Routes.Products,
                async (int? page, int? pageSize, IProductsRepository query, ILogger<CatalogModule> logger,
                    CancellationToken cancellationToken) =>
                {
                    var currentPage = page ?? 1;
                    var currentPageSize = Math.Clamp(pageSize ?? 10, 1, 100);

                    var result = await query.GetAllAsync(currentPage, currentPageSize, cancellationToken);

                    logger.LogInformation("Returning {ProductCount} of {TotalCount} products", result.Items.Count,
                        result.TotalCount);
                    return Results.Ok(result);
                })
            .WithName("GetAllProducts")
            .WithTags("Catalog")
            .AddEndpointFilter<HandlingLogFilter>()
            .Produces<PagedResult<ProductSummaryDto>>();

        endpoints.MapGet(
                Routes.ProductById,
                async (int productId, IProductsRepository query, ILogger<CatalogModule> logger,
                    CancellationToken cancellationToken) =>
                {
                    var product = await query.GetByIdAsync(productId, cancellationToken);

                    if (product is null)
                    {
                        logger.LogWarning("Product {ProductId} not found", productId);
                        return Results.NotFound();
                    }

                    return Results.Ok(product);
                })
            .WithName("GetProductById")
            .WithTags("Catalog")
            .AddEndpointFilter<HandlingLogFilter>()
            .Produces<ProductDetailsDto>()
            .Produces(StatusCodes.Status404NotFound);

        endpoints.MapPost(
                Routes.Products,
                async (ProductRequest request, IProductsRepository repository,
                    CancellationToken cancellationToken) =>
                {
                    var createdProductId = await repository.CreateAsync(request, cancellationToken);

                    return Results.Created(
                        $"/api/catalog/products/{createdProductId}",
                        new { productId = createdProductId });
                })
            .WithName("CreateProduct")
            .WithTags("Catalog")
            .AddEndpointFilter<HandlingLogFilter>()
            .AddEndpointFilter<ValidationFilter<ProductRequest>>()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        endpoints.MapPut(
                Routes.ProductById,
                async (int productId, ProductRequest request, IProductsRepository repository,
                    ILogger<CatalogModule> logger, CancellationToken cancellationToken) =>
                {
                    var updated = await repository.UpdateAsync(productId, request, cancellationToken);

                    if (!updated)
                    {
                        logger.LogWarning("Product {ProductId} not found for update", productId);
                        return Results.NotFound();
                    }

                    return Results.NoContent();
                })
            .WithName("UpdateProduct")
            .WithTags("Catalog")
            .AddEndpointFilter<HandlingLogFilter>()
            .AddEndpointFilter<ValidationFilter<ProductRequest>>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        endpoints.MapPatch(
                Routes.ProductDiscontinue,
                async (int productId, IProductsRepository repository, ILogger<CatalogModule> logger,
                    CancellationToken cancellationToken) =>
                {
                    var discontinued = await repository.DiscontinueAsync(productId, cancellationToken);

                    if (!discontinued)
                    {
                        logger.LogWarning("Product {ProductId} not found for discontinuation", productId);
                        return Results.NotFound();
                    }

                    return Results.NoContent();
                })
            .WithName("DiscontinueProduct")
            .WithTags("Catalog")
            .AddEndpointFilter<HandlingLogFilter>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        endpoints.MapPatch(
                Routes.ProductReinstate,
                async (int productId, IProductsRepository repository, ILogger<CatalogModule> logger,
                    CancellationToken cancellationToken) =>
                {
                    var reinstated = await repository.ReinstateAsync(productId, cancellationToken);

                    if (!reinstated)
                    {
                        logger.LogWarning("Product {ProductId} not found for reinstatement", productId);
                        return Results.NotFound();
                    }

                    return Results.NoContent();
                })
            .WithName("ReinstateProduct")
            .WithTags("Catalog")
            .AddEndpointFilter<HandlingLogFilter>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return endpoints;
    }
}