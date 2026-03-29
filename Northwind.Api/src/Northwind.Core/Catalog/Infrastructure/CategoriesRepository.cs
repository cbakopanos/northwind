using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Northwind.Catalog.Application;
using Northwind.Shared.Abstractions;

namespace Northwind.Catalog.Infrastructure;

public sealed class CategoriesRepository(
    CatalogDbContext dbContext,
    ILogger<CategoriesRepository> logger) : ICategoriesRepository
{
    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching category count from database");

        var count = await dbContext.Categories
            .CountAsync(cancellationToken);

        logger.LogInformation("{CategoryCount} categories found", count);

        return count;
    }

    public async Task<PagedResult<CategorySummaryDto>> GetAllAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching categories from database (page {Page}, pageSize {PageSize})", page, pageSize);

        var totalCount = await dbContext.Categories.CountAsync(cancellationToken);

        var categories = await dbContext.Categories
            .AsNoTracking()
            .OrderBy(x => x.CategoryName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(CategoryMappings.ToSummaryDto)
            .ToListAsync(cancellationToken);

        logger.LogInformation("Fetched {CategoryCount} of {TotalCount} categories from database", categories.Count, totalCount);

        return new PagedResult<CategorySummaryDto>(categories, page, pageSize, totalCount);
    }

    public async Task<CategoryDetailsDto?> GetByIdAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching category {CategoryId} from database", categoryId);

        var category = await dbContext.Categories
            .AsNoTracking()
            .Where(x => x.CategoryId == categoryId)
            .Select(CategoryMappings.ToDetailsDto)
            .SingleOrDefaultAsync(cancellationToken);

        logger.LogInformation(
            category is null
                ? "Category {CategoryId} was not found"
                : "Category {CategoryId} was found",
            categoryId);

        return category;
    }

    public async Task<int> CreateAsync(CategoryRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating category with CategoryName {CategoryName}", request.CategoryName);

        var entity = new CategoryEntity().ToCategoryEntity(request);

        dbContext.Categories.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Created category with CategoryId {CategoryId}", entity.CategoryId);

        return entity.CategoryId;
    }

    public async Task<bool> UpdateAsync(int categoryId, CategoryRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating category {CategoryId}", categoryId);

        var entity = await dbContext.Categories
            .SingleOrDefaultAsync(x => x.CategoryId == categoryId, cancellationToken);

        if (entity is null)
        {
            logger.LogInformation("Category {CategoryId} was not found for update", categoryId);
            return false;
        }

        entity.ToCategoryEntity(request);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Updated category {CategoryId}", categoryId);
        return true;
    }

    public async Task<byte[]?> GetPictureAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching picture for category {CategoryId}", categoryId);

        var picture = await dbContext.Categories
            .AsNoTracking()
            .Where(x => x.CategoryId == categoryId)
            .Select(x => x.Picture)
            .SingleOrDefaultAsync(cancellationToken);

        logger.LogInformation(
            picture is null
                ? "Picture for category {CategoryId} was not found"
                : "Picture for category {CategoryId} was found ({PictureBytes} bytes)",
            categoryId, picture?.Length ?? 0);

        // The Northwind database stores pictures with a 78-byte OLE container
        // header prepended before the actual BMP data. Strip it before serving.
        const int oleHeaderLength = 78;
        if (picture is not null && picture.Length > oleHeaderLength)
            picture = picture[oleHeaderLength..];

        return picture;
    }
}
