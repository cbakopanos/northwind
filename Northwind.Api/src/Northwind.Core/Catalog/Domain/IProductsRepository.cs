using Northwind.Shared.Abstractions;

namespace Northwind.Catalog.Domain;

public interface IProductsRepository
{
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    Task<PagedResult<Product>> GetAllAsync(int page = 1, int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<Product?> GetByIdAsync(ProductId productId, CancellationToken cancellationToken = default);

    Task<ProductId> AddAsync(Product product, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Product product, CancellationToken cancellationToken = default);
}
