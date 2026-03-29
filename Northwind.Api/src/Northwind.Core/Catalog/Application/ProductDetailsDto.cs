namespace Northwind.Catalog.Application;

public sealed record ProductDetailsDto(
    int ProductId,
    string ProductName,
    int? SupplierId,
    int? CategoryId,
    string? CategoryName,
    string? QuantityPerUnit,
    decimal UnitPrice,
    short UnitsInStock,
    short UnitsOnOrder,
    short ReorderLevel,
    bool IsDiscontinued);
