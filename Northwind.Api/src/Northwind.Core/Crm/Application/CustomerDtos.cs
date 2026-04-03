using Northwind.Crm.Domain;
using Northwind.Shared.Application;

namespace Northwind.Crm.Application;

public sealed record CustomerDetailsDto(
    string CustomerId,
    string CompanyName,
    Contact Contact,
    Address Address,
    Communication Communication);

public sealed record CustomerSummaryDto(
    string CustomerId,
    string CompanyName,
    Contact Contact);

public sealed record CustomerRequest(
    string CompanyName,
    Contact? Contact,
    Address? Address,
    Communication? Communication);

public static class CustomerDtoMappings
{
    public static CustomerSummaryDto ToSummaryDto(this Customer customer) => new(
        customer.Id.Value,
        customer.CompanyName.Value,
        new Contact(
            customer.Contact.ContactName,
            customer.Contact.ContactTitle));

    public static CustomerDetailsDto ToDetailsDto(this Customer customer) => new(
        customer.Id.Value,
        customer.CompanyName.Value,
        new Contact(
            customer.Contact.ContactName,
            customer.Contact.ContactTitle),
        new Address(
            customer.Address.AddressLine,
            customer.Address.City,
            customer.Address.Region,
            customer.Address.PostalCode,
            customer.Address.Country),
        new Communication(
            customer.Communication.Phone,
            customer.Communication.Fax,
            customer.Communication.HomepageUrl));
}

