using Northwind.Catalog.Domain;

namespace Northwind.Catalog.Application;

public sealed record ProductDetailsDto(
    int ProductId,
    string ProductName,
    Supplier? Supplier,
    int? CategoryId,
    string? CategoryName,
    string? QuantityPerUnit,
    decimal UnitPrice,
    InventoryLevel Inventory,
    short ReorderLevel,
    bool IsDiscontinued);

public sealed record Supplier(
    int SupplierId,
    string DisplayName);

public sealed record InventoryLevel(
    short UnitsInStock,
    short UnitsOnOrder);

public sealed record ProductRequest(
    string ProductName,
    int? SupplierId,
    int? CategoryId,
    string? QuantityPerUnit,
    decimal UnitPrice,
    InventoryLevel? Inventory,
    short ReorderLevel);

public sealed record ProductSummaryDto(
    int ProductId,
    string ProductName,
    string? CategoryName,
    decimal UnitPrice,
    bool IsDiscontinued);

public static class ProductDtoMappings
{
    static int GetPersistedProductId(Product product) =>
        product.Id?.Value ?? throw new InvalidOperationException("Product id is not assigned.");

    public static ProductSummaryDto ToSummaryDto(this Product product) => new(
        GetPersistedProductId(product),
        product.ProductName.Value,
        product.CategoryName,
        product.UnitPrice.Value,
        product.IsDiscontinued);

    public static ProductDetailsDto ToDetailsDto(this Product product)
    {
        Supplier? supplier = null;
        if (product.SupplierId.HasValue && !string.IsNullOrWhiteSpace(product.SupplierDisplayName))
            supplier = new Supplier(product.SupplierId.Value, product.SupplierDisplayName);

        return new ProductDetailsDto(
            GetPersistedProductId(product),
            product.ProductName.Value,
            supplier,
            product.CategoryId,
            product.CategoryName,
            product.QuantityPerUnit.Value,
            product.UnitPrice.Value,
            new InventoryLevel(product.Inventory.UnitsInStock, product.Inventory.UnitsOnOrder),
            product.ReorderLevel.Value,
            product.IsDiscontinued);
    }
}