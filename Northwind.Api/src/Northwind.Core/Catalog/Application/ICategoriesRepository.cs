using Northwind.Shared.Abstractions;

namespace Northwind.Catalog.Application;

public interface ICategoriesRepository
{
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    Task<PagedResult<CategorySummaryDto>> GetAllAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);

    Task<CategoryDetailsDto?> GetByIdAsync(int categoryId, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CategoryRequest request, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(int categoryId, CategoryRequest request, CancellationToken cancellationToken = default);

    Task<byte[]?> GetPictureAsync(int categoryId, CancellationToken cancellationToken = default);
}
