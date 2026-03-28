namespace Northwind.Supplier.Application;

public sealed record SupplierListItem(
    int SupplierId,
    string CompanyName,
    string? ContactName,
    string? ContactTitle,
    string? City,
    string? Country,
    string? Phone);
