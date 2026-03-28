namespace Northwind.Supplier.Application;

public sealed record SupplierListItem(
    int SupplierId,
    string CompanyName,
    string? ContactName,
    string? ContactTitle,
    string? Address,
    string? City,
    string? Region,
    string? PostalCode,
    string? Country,
    string? Phone,
    string? Fax,
    string? HomepageUrl);
