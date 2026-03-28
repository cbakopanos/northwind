namespace Northwind.Supplier.Application;

public sealed record CreateSupplierRequest(
    string CompanyName,
    SupplierContact? Contact,
    Address? Address,
    SupplierCommunication? Communication);
