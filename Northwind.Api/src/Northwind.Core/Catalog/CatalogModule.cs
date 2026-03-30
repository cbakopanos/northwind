using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Northwind.Catalog.Application;
using Northwind.Catalog.Infrastructure;
using Northwind.Shared.Abstractions;

namespace Northwind.Catalog;

[Module]
[Infrastructure(typeof(CatalogDbContext))]
[Repository(typeof(ICategoriesRepository), typeof(CategoriesRepository))]
[Repository(typeof(IProductsRepository), typeof(ProductsRepository))]
public sealed class CatalogModule : IModule
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/catalog/health", async (ICategoriesRepository categories, IProductsRepository products, ILogger<CatalogModule> logger, CancellationToken cancellationToken) =>
        {
            logger.LogInformation("Health endpoint requested for module {ModuleContext}", "Catalog");
            var categoryCount = await categories.GetCountAsync(cancellationToken);
            var productCount = await products.GetCountAsync(cancellationToken);
            return Results.Ok(new { context = "Catalog", status = "ok", categories = categoryCount, products = productCount });
        });

        // ── Categories ──────────────────────────────────────────────────────

        endpoints.MapGet(
            "/api/catalog/categories",
            async (ICategoriesRepository query, ILogger<CatalogModule> logger, CancellationToken cancellationToken) =>
            {
                logger.LogInformation("Handling {Endpoint}", "GET /api/catalog/categories");

                var categories = await query.GetAllAsync(cancellationToken);

                logger.LogInformation("Returning {CategoryCount} categories", categories.Count);
                return Results.Ok(categories);
            })
            .WithName("GetAllCategories")
            .WithTags("Catalog")
            .Produces<IReadOnlyList<CategorySummaryDto>>(StatusCodes.Status200OK);

        endpoints.MapGet(
            "/api/catalog/categories/{categoryId:int}",
            async (int categoryId, ICategoriesRepository query, ILogger<CatalogModule> logger, CancellationToken cancellationToken) =>
            {
                logger.LogInformation("Handling {Endpoint} for CategoryId {CategoryId}", "GET /api/catalog/categories/{categoryId:int}", categoryId);

                var category = await query.GetByIdAsync(categoryId, cancellationToken);

                if (category is null)
                {
                    logger.LogInformation("Category {CategoryId} not found", categoryId);
                    return Results.NotFound();
                }

                logger.LogInformation("Returning category {CategoryId}", categoryId);
                return Results.Ok(category);
            })
            .WithName("GetCategoryById")
            .WithTags("Catalog")
            .Produces<CategoryDetailsDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        endpoints.MapGet(
            "/api/catalog/categories/{categoryId:int}/picture",
            async (int categoryId, ICategoriesRepository query, ILogger<CatalogModule> logger, CancellationToken cancellationToken) =>
            {
                logger.LogInformation("Handling {Endpoint} for CategoryId {CategoryId}", "GET /api/catalog/categories/{categoryId:int}/picture", categoryId);

                var picture = await query.GetPictureAsync(categoryId, cancellationToken);

                if (picture is null)
                {
                    logger.LogInformation("Picture for category {CategoryId} not found", categoryId);
                    return Results.NotFound();
                }

                logger.LogInformation("Returning picture for category {CategoryId} ({PictureBytes} bytes)", categoryId, picture.Length);
                return Results.File(picture, "image/bmp");
            })
            .WithName("GetCategoryPicture")
            .WithTags("Catalog")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        endpoints.MapPost(
            "/api/catalog/categories",
            async (CategoryRequest request, ICategoriesRepository repository, ILogger<CatalogModule> logger, CancellationToken cancellationToken) =>
            {
                logger.LogInformation("Handling {Endpoint}", "POST /api/catalog/categories");

                if (string.IsNullOrWhiteSpace(request.CategoryName))
                {
                    logger.LogInformation("Create category request rejected: CategoryName is required");
                    return Results.BadRequest(new { error = "CategoryName is required." });
                }

                var createdCategoryId = await repository.CreateAsync(request, cancellationToken);

                logger.LogInformation("Created category {CategoryId}", createdCategoryId);
                return Results.Created(
                    $"/api/catalog/categories/{createdCategoryId}",
                    new { categoryId = createdCategoryId });
            })
            .WithName("CreateCategory")
            .WithTags("Catalog")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        endpoints.MapPut(
            "/api/catalog/categories/{categoryId:int}",
            async (int categoryId, CategoryRequest request, ICategoriesRepository repository, ILogger<CatalogModule> logger, CancellationToken cancellationToken) =>
            {
                logger.LogInformation("Handling {Endpoint} for CategoryId {CategoryId}", "PUT /api/catalog/categories/{categoryId:int}", categoryId);

                if (string.IsNullOrWhiteSpace(request.CategoryName))
                {
                    logger.LogInformation("Update category request rejected for CategoryId {CategoryId}: CategoryName is required", categoryId);
                    return Results.BadRequest(new { error = "CategoryName is required." });
                }

                var updated = await repository.UpdateAsync(categoryId, request, cancellationToken);

                if (!updated)
                {
                    logger.LogInformation("Category {CategoryId} not found for update", categoryId);
                    return Results.NotFound();
                }

                logger.LogInformation("Updated category {CategoryId}", categoryId);
                return Results.NoContent();
            })
            .WithName("UpdateCategory")
            .WithTags("Catalog")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        // ── Products ────────────────────────────────────────────────────────

        endpoints.MapGet(
            "/api/catalog/products",
            async (int? page, int? pageSize, IProductsRepository query, ILogger<CatalogModule> logger, CancellationToken cancellationToken) =>
            {
                var currentPage = page ?? 1;
                var currentPageSize = Math.Clamp(pageSize ?? 10, 1, 100);
                logger.LogInformation("Handling {Endpoint} (page {Page}, pageSize {PageSize})", "GET /api/catalog/products", currentPage, currentPageSize);

                var result = await query.GetAllAsync(currentPage, currentPageSize, cancellationToken);

                logger.LogInformation("Returning {ProductCount} of {TotalCount} products", result.Items.Count, result.TotalCount);
                return Results.Ok(result);
            })
            .WithName("GetAllProducts")
            .WithTags("Catalog")
            .Produces<PagedResult<ProductSummaryDto>>(StatusCodes.Status200OK);

        endpoints.MapGet(
            "/api/catalog/products/{productId:int}",
            async (int productId, IProductsRepository query, ILogger<CatalogModule> logger, CancellationToken cancellationToken) =>
            {
                logger.LogInformation("Handling {Endpoint} for ProductId {ProductId}", "GET /api/catalog/products/{productId:int}", productId);

                var product = await query.GetByIdAsync(productId, cancellationToken);

                if (product is null)
                {
                    logger.LogInformation("Product {ProductId} not found", productId);
                    return Results.NotFound();
                }

                logger.LogInformation("Returning product {ProductId}", productId);
                return Results.Ok(product);
            })
            .WithName("GetProductById")
            .WithTags("Catalog")
            .Produces<ProductDetailsDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        endpoints.MapPost(
            "/api/catalog/products",
            async (ProductRequest request, IProductsRepository repository, ILogger<CatalogModule> logger, CancellationToken cancellationToken) =>
            {
                logger.LogInformation("Handling {Endpoint}", "POST /api/catalog/products");

                if (string.IsNullOrWhiteSpace(request.ProductName))
                {
                    logger.LogInformation("Create product request rejected: ProductName is required");
                    return Results.BadRequest(new { error = "ProductName is required." });
                }

                var createdProductId = await repository.CreateAsync(request, cancellationToken);

                logger.LogInformation("Created product {ProductId}", createdProductId);
                return Results.Created(
                    $"/api/catalog/products/{createdProductId}",
                    new { productId = createdProductId });
            })
            .WithName("CreateProduct")
            .WithTags("Catalog")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        endpoints.MapPut(
            "/api/catalog/products/{productId:int}",
            async (int productId, ProductRequest request, IProductsRepository repository, ILogger<CatalogModule> logger, CancellationToken cancellationToken) =>
            {
                logger.LogInformation("Handling {Endpoint} for ProductId {ProductId}", "PUT /api/catalog/products/{productId:int}", productId);

                if (string.IsNullOrWhiteSpace(request.ProductName))
                {
                    logger.LogInformation("Update product request rejected for ProductId {ProductId}: ProductName is required", productId);
                    return Results.BadRequest(new { error = "ProductName is required." });
                }

                var updated = await repository.UpdateAsync(productId, request, cancellationToken);

                if (!updated)
                {
                    logger.LogInformation("Product {ProductId} not found for update", productId);
                    return Results.NotFound();
                }

                logger.LogInformation("Updated product {ProductId}", productId);
                return Results.NoContent();
            })
            .WithName("UpdateProduct")
            .WithTags("Catalog")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        endpoints.MapPatch(
            "/api/catalog/products/{productId:int}/discontinue",
            async (int productId, IProductsRepository repository, ILogger<CatalogModule> logger, CancellationToken cancellationToken) =>
            {
                logger.LogInformation("Handling {Endpoint} for ProductId {ProductId}", "PATCH /api/catalog/products/{productId:int}/discontinue", productId);

                var discontinued = await repository.DiscontinueAsync(productId, cancellationToken);

                if (!discontinued)
                {
                    logger.LogInformation("Product {ProductId} not found for discontinuation", productId);
                    return Results.NotFound();
                }

                logger.LogInformation("Discontinued product {ProductId}", productId);
                return Results.NoContent();
            })
            .WithName("DiscontinueProduct")
            .WithTags("Catalog")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        endpoints.MapPatch(
            "/api/catalog/products/{productId:int}/reinstate",
            async (int productId, IProductsRepository repository, ILogger<CatalogModule> logger, CancellationToken cancellationToken) =>
            {
                logger.LogInformation("Handling {Endpoint} for ProductId {ProductId}", "PATCH /api/catalog/products/{productId:int}/reinstate", productId);

                var reinstated = await repository.ReinstateAsync(productId, cancellationToken);

                if (!reinstated)
                {
                    logger.LogInformation("Product {ProductId} not found for reinstatement", productId);
                    return Results.NotFound();
                }

                logger.LogInformation("Reinstated product {ProductId}", productId);
                return Results.NoContent();
            })
            .WithName("ReinstateProduct")
            .WithTags("Catalog")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return endpoints;
    }
}
