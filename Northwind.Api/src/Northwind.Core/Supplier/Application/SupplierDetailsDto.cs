namespace Northwind.Supplier.Application;

public sealed record SupplierDetailsDto(
    int SupplierId,
    string CompanyName,
    SupplierContact Contact,
    Address Address,
    SupplierCommunication Communication);