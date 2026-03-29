namespace Northwind.Catalog.Application;

public sealed record ProductDetailsDto(
    int ProductId,
    string ProductName,
    int? SupplierId,
    int? CategoryId,
    string? CategoryName,
    string? QuantityPerUnit,
    decimal UnitPrice,
    InventoryLevel Inventory,
    short ReorderLevel,
    bool IsDiscontinued);
