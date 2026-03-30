namespace Northwind.Catalog.Application;

public sealed record ProductRequest(
    string ProductName,
    int? SupplierId,
    int? CategoryId,
    string? QuantityPerUnit,
    decimal UnitPrice,
    InventoryLevel? Inventory,
    short ReorderLevel);