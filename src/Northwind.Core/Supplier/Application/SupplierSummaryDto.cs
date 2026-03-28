namespace Northwind.Supplier.Application;

public sealed record SupplierSummaryDto(
    int SupplierId,
    string CompanyName,
    string? ContactName,
    string? ContactTitle);
