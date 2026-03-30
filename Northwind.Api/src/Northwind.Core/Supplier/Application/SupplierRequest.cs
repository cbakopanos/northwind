namespace Northwind.Supplier.Application;

public sealed record SupplierRequest(
    string CompanyName,
    SupplierContact? Contact,
    Address? Address,
    SupplierCommunication? Communication);