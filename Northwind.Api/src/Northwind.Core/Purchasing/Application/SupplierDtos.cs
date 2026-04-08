using Northwind.Purchasing.Domain;
using Northwind.Shared.Application;

namespace Northwind.Purchasing.Application;

public sealed record SupplierDetailsDto(
    int SupplierId,
    string CompanyName,
    Contact Contact,
    Address Address,
    Communication Communication);

public sealed record SupplierSummaryDto(
    int SupplierId,
    string CompanyName,
    Contact Contact);

public sealed record SupplierRequest(
    string CompanyName,
    Contact? Contact,
    Address? Address,
    Communication? Communication);

public static class SupplierDtoMappings
{
    static int GetPersistedSupplierId(Supplier supplier) =>
        supplier.Id?.Value ?? throw new InvalidOperationException("Supplier id is not assigned.");

    public static SupplierSummaryDto ToSummaryDto(this Supplier supplier) => new(
        GetPersistedSupplierId(supplier),
        supplier.CompanyName.Value,
        new Contact(
            supplier.Contact.ContactName,
            supplier.Contact.ContactTitle));

    public static SupplierDetailsDto ToDetailsDto(this Supplier supplier) => new(
        GetPersistedSupplierId(supplier),
        supplier.CompanyName.Value,
        new Contact(
            supplier.Contact.ContactName,
            supplier.Contact.ContactTitle),
        new Address(
            supplier.Address.AddressLine,
            supplier.Address.City,
            supplier.Address.Region,
            supplier.Address.PostalCode,
            supplier.Address.Country),
        new Communication(
            supplier.Communication.Phone,
            supplier.Communication.Fax,
            supplier.Communication.HomepageUrl));
}

