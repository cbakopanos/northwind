using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Northwind.Catalog.Application;
using Northwind.Shared.Abstractions;

namespace Northwind.Catalog.Infrastructure;

public sealed class ProductsRepository(
    CatalogDbContext dbContext,
    ILogger<ProductsRepository> logger) : IProductsRepository
{
    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching product count from database");

        var count = await dbContext.Products
            .CountAsync(cancellationToken);

        logger.LogInformation("{ProductCount} products found", count);

        return count;
    }

    public async Task<PagedResult<ProductSummaryDto>> GetAllAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching products from database (page {Page}, pageSize {PageSize})", page, pageSize);

        var totalCount = await dbContext.Products.CountAsync(cancellationToken);

        var products = await dbContext.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .OrderBy(x => x.ProductName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ProductMappings.ToSummaryDto)
            .ToListAsync(cancellationToken);

        logger.LogInformation("Fetched {ProductCount} of {TotalCount} products from database", products.Count, totalCount);

        return new PagedResult<ProductSummaryDto>(products, page, pageSize, totalCount);
    }

    public async Task<ProductDetailsDto?> GetByIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching product {ProductId} from database", productId);

        var product = await dbContext.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .Where(x => x.ProductId == productId)
            .Select(ProductMappings.ToDetailsDto)
            .SingleOrDefaultAsync(cancellationToken);

        logger.LogInformation(
            product is null
                ? "Product {ProductId} was not found"
                : "Product {ProductId} was found",
            productId);

        return product;
    }

    public async Task<int> CreateAsync(ProductRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating product with ProductName {ProductName}", request.ProductName);

        var entity = new ProductEntity().ToProductEntity(request);

        dbContext.Products.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Created product with ProductId {ProductId}", entity.ProductId);

        return entity.ProductId;
    }

    public async Task<bool> UpdateAsync(int productId, ProductRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating product {ProductId}", productId);

        var entity = await dbContext.Products
            .SingleOrDefaultAsync(x => x.ProductId == productId, cancellationToken);

        if (entity is null)
        {
            logger.LogInformation("Product {ProductId} was not found for update", productId);
            return false;
        }

        entity.ToProductEntity(request);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Updated product {ProductId}", productId);
        return true;
    }

    public async Task<bool> DiscontinueAsync(int productId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Discontinuing product {ProductId}", productId);

        var entity = await dbContext.Products
            .SingleOrDefaultAsync(x => x.ProductId == productId, cancellationToken);

        if (entity is null)
        {
            logger.LogInformation("Product {ProductId} was not found for discontinuation", productId);
            return false;
        }

        entity.IsDiscontinued = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Discontinued product {ProductId}", productId);
        return true;
    }

    public async Task<bool> ReinstateAsync(int productId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Reinstating product {ProductId}", productId);

        var entity = await dbContext.Products
            .SingleOrDefaultAsync(x => x.ProductId == productId, cancellationToken);

        if (entity is null)
        {
            logger.LogInformation("Product {ProductId} was not found for reinstatement", productId);
            return false;
        }

        entity.IsDiscontinued = false;
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Reinstated product {ProductId}", productId);
        return true;
    }
}
