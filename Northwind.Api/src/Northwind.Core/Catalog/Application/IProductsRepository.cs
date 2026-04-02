using Northwind.Shared.Abstractions;
using Northwind.Shared.Application;

namespace Northwind.Catalog.Application;

public interface IProductsRepository : IBaseRepository
{
    Task<PagedResult<ProductSummaryDto>> GetAllAsync(int page = 1, int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<ProductDetailsDto?> GetByIdAsync(int productId, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(ProductRequest request, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(int productId, ProductRequest request, CancellationToken cancellationToken = default);

    Task<bool> DiscontinueAsync(int productId, CancellationToken cancellationToken = default);

    Task<bool> ReinstateAsync(int productId, CancellationToken cancellationToken = default);
}