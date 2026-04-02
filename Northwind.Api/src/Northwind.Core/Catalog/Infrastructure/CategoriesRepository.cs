using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Northwind.Catalog.Application;
using Northwind.Shared.Infrastructure;

namespace Northwind.Catalog.Infrastructure;

public sealed class CategoriesRepository(
    CatalogDbContext dbContext,
    ILogger<CategoriesRepository> logger) : BaseRepository<CategoryEntity>(dbContext, logger), ICategoriesRepository
{
    public async Task<IReadOnlyList<CategorySummaryDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching categories from database");

        var categories = await dbContext.Categories
            .AsNoTracking()
            .OrderByDescending(x => x.CategoryId)
            .Select(ToSummaryDto)
            .ToListAsync(cancellationToken);

        logger.LogInformation("Fetched {CategoryCount} categories from database", categories.Count);

        return categories;
    }

    public async Task<CategoryDetailsDto?> GetByIdAsync(int categoryId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching category {CategoryId} from database", categoryId);

        var category = await dbContext.Categories
            .AsNoTracking()
            .Where(x => x.CategoryId == categoryId)
            .Select(ToDetailsDto)
            .SingleOrDefaultAsync(cancellationToken);

        logger.LogInformation(
            category is null
                ? "Category {CategoryId} was not found"
                : "Category {CategoryId} was found",
            categoryId);

        return category;
    }

    public async Task<int> CreateAsync(CategoryRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating category with CategoryName {CategoryName}", request.CategoryName);

        var entity = ToCategoryEntity(new CategoryEntity(), request);

        dbContext.Categories.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Created category with CategoryId {CategoryId}", entity.CategoryId);

        return entity.CategoryId;
    }

    public async Task<bool> UpdateAsync(int categoryId, CategoryRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating category {CategoryId}", categoryId);

        var entity = await dbContext.Categories
            .SingleOrDefaultAsync(x => x.CategoryId == categoryId, cancellationToken);

        if (entity is null)
        {
            logger.LogInformation("Category {CategoryId} was not found for update", categoryId);
            return false;
        }

        ToCategoryEntity(entity, request);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Updated category {CategoryId}", categoryId);
        return true;
    }

    public async Task<byte[]?> GetPictureAsync(int categoryId, CancellationToken cancellationToken)
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

    static readonly Expression<Func<CategoryEntity, CategorySummaryDto>> ToSummaryDto =
        x => new CategorySummaryDto(
            x.CategoryId,
            x.CategoryName,
            x.Picture != null);

    static readonly Expression<Func<CategoryEntity, CategoryDetailsDto>> ToDetailsDto =
        x => new CategoryDetailsDto(
            x.CategoryId,
            x.CategoryName,
            x.Description,
            x.Picture != null);
    static CategoryEntity ToCategoryEntity(CategoryEntity entity, CategoryRequest request)
    {
        entity.CategoryName = request.CategoryName.Trim();
        entity.Description = request.Description;

        return entity;
    }
}