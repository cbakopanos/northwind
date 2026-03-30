namespace Northwind.Supplier.Application;

public sealed record SupplierSummaryDto(
    int SupplierId,
    string CompanyName,
    SupplierContact Contact);