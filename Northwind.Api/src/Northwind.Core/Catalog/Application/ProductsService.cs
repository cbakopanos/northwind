using Northwind.Catalog.Domain;
using Northwind.Shared.Abstractions;

namespace Northwind.Catalog.Application;

public sealed class ProductsService(IProductsRepository repository) : IProductsService
{
    public Task<int> GetCountAsync(CancellationToken cancellationToken) => repository.GetCountAsync(cancellationToken);

    public async Task<PagedResult<ProductSummaryDto>> GetAllAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        var products = await repository.GetAllAsync(page, pageSize, cancellationToken);

        var items = products.Items
            .Select(product => product.ToSummaryDto())
            .ToList();

        return new PagedResult<ProductSummaryDto>(items, products.Page, products.PageSize, products.TotalCount);
    }

    public async Task<ProductDetailsDto?> GetByIdAsync(int productId, CancellationToken cancellationToken)
    {
        var id = ProductId.Create(productId);
        var product = await repository.GetByIdAsync(id, cancellationToken);
        return product?.ToDetailsDto();
    }

    public async Task<int> CreateAsync(ProductRequest request, CancellationToken cancellationToken)
    {
        var product = Product.Create(
            request.ProductName,
            request.SupplierId,
            request.CategoryId,
            request.QuantityPerUnit,
            request.UnitPrice,
            request.Inventory?.UnitsInStock ?? 0,
            request.Inventory?.UnitsOnOrder ?? 0,
            request.ReorderLevel);

        var id = await repository.AddAsync(product, cancellationToken);
        return id.Value;
    }

    public async Task<bool> UpdateAsync(int productId, ProductRequest request, CancellationToken cancellationToken)
    {
        var id = ProductId.Create(productId);
        var product = await repository.GetByIdAsync(id, cancellationToken);

        if (product is null)
            return false;

        product.UpdateDetails(
            request.ProductName,
            request.SupplierId,
            request.CategoryId,
            request.QuantityPerUnit,
            request.UnitPrice,
            request.Inventory?.UnitsInStock ?? 0,
            request.Inventory?.UnitsOnOrder ?? 0,
            request.ReorderLevel);

        return await repository.UpdateAsync(product, cancellationToken);
    }

    public async Task<bool> DiscontinueAsync(int productId, CancellationToken cancellationToken)
    {
        var id = ProductId.Create(productId);
        var product = await repository.GetByIdAsync(id, cancellationToken);

        if (product is null)
            return false;

        product.Discontinue();
        return await repository.UpdateAsync(product, cancellationToken);
    }

    public async Task<bool> ReinstateAsync(int productId, CancellationToken cancellationToken)
    {
        var id = ProductId.Create(productId);
        var product = await repository.GetByIdAsync(id, cancellationToken);

        if (product is null)
            return false;

        product.Reinstate();
        return await repository.UpdateAsync(product, cancellationToken);
    }
}
