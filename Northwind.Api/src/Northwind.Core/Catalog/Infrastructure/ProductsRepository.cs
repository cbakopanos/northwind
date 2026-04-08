using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Northwind.Catalog.Domain;
using Northwind.Shared.Abstractions;
using Northwind.Shared.Infrastructure;

namespace Northwind.Catalog.Infrastructure;

public sealed class ProductsRepository(
    CatalogDbContext dbContext,
    ILogger<ProductsRepository> logger) : BaseRepository<ProductEntity>(dbContext, logger), IProductsRepository
{
    public async Task<PagedResult<Product>> GetAllAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching products from database (page {Page}, pageSize {PageSize})", page, pageSize);

        var totalCount = await dbContext.Products.CountAsync(cancellationToken);

        var products = await dbContext.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Supplier)
            .OrderBy(x => x.CreatedAt)
            .ThenBy(x => x.ProductId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var domainProducts = products
            .Select(ToDomain)
            .ToList();

        logger.LogInformation("Fetched {ProductCount} of {TotalCount} products from database", products.Count,
            totalCount);

        return new PagedResult<Product>(domainProducts, page, pageSize, totalCount);
    }

    public async Task<Product?> GetByIdAsync(ProductId productId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching product {ProductId} from database", productId.Value);

        var entity = await dbContext.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Supplier)
            .Where(x => x.ProductId == productId.Value)
            .SingleOrDefaultAsync(cancellationToken);

        if (entity is null)
        {
            logger.LogInformation("Product {ProductId} was not found", productId.Value);
            return null;
        }

        logger.LogInformation("Product {ProductId} was found", productId.Value);

        return ToDomain(entity);
    }

    public async Task<ProductId> AddAsync(Product product, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating product with ProductName {ProductName}", product.ProductName.Value);

        var entity = ToProductEntity(new ProductEntity(), product);

        dbContext.Products.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        var id = ProductId.FromPersistence(entity.ProductId);
        product.AssignId(id);

        logger.LogInformation("Created product with ProductId {ProductId}", entity.ProductId);

        return id;
    }

    public async Task<bool> UpdateAsync(Product product,
        CancellationToken cancellationToken)
    {
        var productId = product.Id?.Value ?? throw new InvalidOperationException("Product id is not assigned.");

        logger.LogInformation("Updating product {ProductId}", productId);

        var entity = await dbContext.Products
            .SingleOrDefaultAsync(x => x.ProductId == productId, cancellationToken);

        if (entity is null)
        {
            logger.LogInformation("Product {ProductId} was not found for update", productId);
            return false;
        }

        ToProductEntity(entity, product);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Updated product {ProductId}", productId);
        return true;
    }

    static Product ToDomain(ProductEntity x)
    {
        var supplierDisplayName = x.Supplier is null
            ? null
            : string.Join(", ", new[] { x.Supplier.CompanyName, x.Supplier.ContactName, x.Supplier.ContactTitle }
                .Where(s => !string.IsNullOrWhiteSpace(s)));

        return Product.Rehydrate(
            x.ProductId,
            x.ProductName,
            x.SupplierId,
            x.CategoryId,
            x.QuantityPerUnit,
            x.UnitPrice,
            x.UnitsInStock,
            x.UnitsOnOrder,
            x.ReorderLevel,
            x.IsDiscontinued,
            x.Category?.CategoryName,
            supplierDisplayName);
    }

    static ProductEntity ToProductEntity(ProductEntity entity, Product product)
    {
        entity.ProductName = product.ProductName.Value;
        entity.SupplierId = product.SupplierId;
        entity.CategoryId = product.CategoryId;
        entity.QuantityPerUnit = product.QuantityPerUnit.Value;
        entity.UnitPrice = product.UnitPrice.Value;
        entity.UnitsInStock = product.Inventory.UnitsInStock;
        entity.UnitsOnOrder = product.Inventory.UnitsOnOrder;
        entity.ReorderLevel = product.ReorderLevel.Value;
        entity.IsDiscontinued = product.IsDiscontinued;

        return entity;
    }
}