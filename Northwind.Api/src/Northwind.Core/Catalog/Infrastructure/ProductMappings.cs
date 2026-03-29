using System.Linq.Expressions;
using Northwind.Catalog.Application;

namespace Northwind.Catalog.Infrastructure;

public static class ProductMappings
{
    public static ProductEntity ToProductEntity(this ProductEntity entity, ProductRequest request)
    {
        entity.ProductName = request.ProductName.Trim();
        entity.SupplierId = request.SupplierId;
        entity.CategoryId = request.CategoryId;
        entity.QuantityPerUnit = request.QuantityPerUnit;
        entity.UnitPrice = request.UnitPrice;
        entity.UnitsInStock = request.UnitsInStock;
        entity.UnitsOnOrder = request.UnitsOnOrder;
        entity.ReorderLevel = request.ReorderLevel;

        return entity;
    }

    public static readonly Expression<Func<ProductEntity, ProductSummaryDto>> ToSummaryDto =
        x => new ProductSummaryDto(
            x.ProductId,
            x.ProductName,
            x.Category != null ? x.Category.CategoryName : null,
            x.UnitPrice,
            x.IsDiscontinued);

    public static readonly Expression<Func<ProductEntity, ProductDetailsDto>> ToDetailsDto =
        x => new ProductDetailsDto(
            x.ProductId,
            x.ProductName,
            x.SupplierId,
            x.CategoryId,
            x.Category != null ? x.Category.CategoryName : null,
            x.QuantityPerUnit,
            x.UnitPrice,
            x.UnitsInStock,
            x.UnitsOnOrder,
            x.ReorderLevel,
            x.IsDiscontinued);
}
