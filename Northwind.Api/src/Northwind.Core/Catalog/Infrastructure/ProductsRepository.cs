using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Northwind.Catalog.Application;
using Northwind.Shared.Abstractions;
using Northwind.Shared.Infrastructure;

namespace Northwind.Catalog.Infrastructure;

public sealed class ProductsRepository(
    CatalogDbContext dbContext,
    ILogger<ProductsRepository> logger) : BaseRepository<ProductEntity>(dbContext, logger), IProductsRepository
{
    public async Task<PagedResult<ProductSummaryDto>> GetAllAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching products from database (page {Page}, pageSize {PageSize})", page, pageSize);

        var totalCount = await dbContext.Products.CountAsync(cancellationToken);

        var products = await dbContext.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .OrderBy(x => x.CreatedAt)            
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToSummaryDto)
            .ToListAsync(cancellationToken);

        logger.LogInformation("Fetched {ProductCount} of {TotalCount} products from database", products.Count,
            totalCount);

        return new PagedResult<ProductSummaryDto>(products, page, pageSize, totalCount);
    }

    public async Task<ProductDetailsDto?> GetByIdAsync(int productId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching product {ProductId} from database", productId);

        var entity = await dbContext.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Supplier)
            .Where(x => x.ProductId == productId)
            .SingleOrDefaultAsync(cancellationToken);

        if (entity is null)
        {
            logger.LogInformation("Product {ProductId} was not found", productId);
            return null;
        }

        logger.LogInformation("Product {ProductId} was found", productId);

        return ToDetailsDto(entity);
    }

    public async Task<int> CreateAsync(ProductRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating product with ProductName {ProductName}", request.ProductName);

        var entity = ToProductEntity(new ProductEntity(), request);

        dbContext.Products.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Created product with ProductId {ProductId}", entity.ProductId);

        return entity.ProductId;
    }

    public async Task<bool> UpdateAsync(int productId, ProductRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating product {ProductId}", productId);

        var entity = await dbContext.Products
            .SingleOrDefaultAsync(x => x.ProductId == productId, cancellationToken);

        if (entity is null)
        {
            logger.LogInformation("Product {ProductId} was not found for update", productId);
            return false;
        }

        ToProductEntity(entity, request);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Updated product {ProductId}", productId);
        return true;
    }

    public async Task<bool> DiscontinueAsync(int productId, CancellationToken cancellationToken)
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

    public async Task<bool> ReinstateAsync(int productId, CancellationToken cancellationToken)
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

    static readonly Expression<Func<ProductEntity, ProductSummaryDto>> ToSummaryDto =
        x => new ProductSummaryDto(
            x.ProductId,
            x.ProductName,
            x.Category != null ? x.Category.CategoryName : null,
            x.UnitPrice,
            x.IsDiscontinued);

    static ProductDetailsDto ToDetailsDto(ProductEntity x)
    {
        var supplier = x.Supplier is null ? null : new Supplier(
            x.Supplier.SupplierId,
            string.Join(", ", new[] { x.Supplier.CompanyName, x.Supplier.ContactName, x.Supplier.ContactTitle }
                .Where(s => s is not null)));

        return new ProductDetailsDto(
            x.ProductId,
            x.ProductName,
            supplier,
            x.CategoryId,
            x.Category?.CategoryName,
            x.QuantityPerUnit,
            x.UnitPrice,
            new InventoryLevel(x.UnitsInStock, x.UnitsOnOrder),
            x.ReorderLevel,
            x.IsDiscontinued);
    }

    static ProductEntity ToProductEntity(ProductEntity entity, ProductRequest request)
    {
        entity.ProductName = request.ProductName.Trim();
        entity.SupplierId = request.SupplierId;
        entity.CategoryId = request.CategoryId;
        entity.QuantityPerUnit = request.QuantityPerUnit;
        entity.UnitPrice = request.UnitPrice;
        entity.UnitsInStock = request.Inventory?.UnitsInStock ?? 0;
        entity.UnitsOnOrder = request.Inventory?.UnitsOnOrder ?? 0;
        entity.ReorderLevel = request.ReorderLevel;

        return entity;
    }
}