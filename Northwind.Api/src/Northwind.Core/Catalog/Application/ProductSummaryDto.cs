namespace Northwind.Catalog.Application;

public sealed record ProductSummaryDto(
    int ProductId,
    string ProductName,
    string? CategoryName,
    decimal UnitPrice,
    bool IsDiscontinued);