using Northwind.Shared.Abstractions;

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
    short ReorderLevel) : IValidatable
{
    public IReadOnlyList<string> Validate()
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(ProductName))
            errors.Add("ProductName is required.");
        return errors;
    }
}

public sealed record ProductSummaryDto(
    int ProductId,
    string ProductName,
    string? CategoryName,
    decimal UnitPrice,
    bool IsDiscontinued);