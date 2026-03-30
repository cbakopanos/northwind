namespace Northwind.Catalog.Application;

public interface ICategoriesRepository
{
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CategorySummaryDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<CategoryDetailsDto?> GetByIdAsync(int categoryId, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CategoryRequest request, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(int categoryId, CategoryRequest request, CancellationToken cancellationToken = default);

    Task<byte[]?> GetPictureAsync(int categoryId, CancellationToken cancellationToken = default);
}