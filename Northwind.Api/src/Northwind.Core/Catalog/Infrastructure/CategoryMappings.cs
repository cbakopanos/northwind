using System.Linq.Expressions;
using Northwind.Catalog.Application;

namespace Northwind.Catalog.Infrastructure;

public static class CategoryMappings
{
    public static CategoryEntity ToCategoryEntity(this CategoryEntity entity, CategoryRequest request)
    {
        entity.CategoryName = request.CategoryName.Trim();
        entity.Description = request.Description;

        return entity;
    }

    public static readonly Expression<Func<CategoryEntity, CategorySummaryDto>> ToSummaryDto =
        x => new CategorySummaryDto(
            x.CategoryId,
            x.CategoryName);

    public static readonly Expression<Func<CategoryEntity, CategoryDetailsDto>> ToDetailsDto =
        x => new CategoryDetailsDto(
            x.CategoryId,
            x.CategoryName,
            x.Description,
            x.Picture != null);
}
