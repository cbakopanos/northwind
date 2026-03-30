using System.Linq.Expressions;
using Northwind.Crm.Application;

namespace Northwind.Crm.Infrastructure;

public static class CustomerMappings
{
    public static readonly Expression<Func<CustomerEntity, CustomerSummaryDto>> ToSummaryDto =
        x => new CustomerSummaryDto(
            x.CustomerId,
            x.CompanyName,
            new CustomerContact(
                x.ContactName,
                x.ContactTitle));

    public static readonly Expression<Func<CustomerEntity, CustomerDetailsDto>> ToDetailsDto =
        x => new CustomerDetailsDto(
            x.CustomerId,
            x.CompanyName,
            new CustomerContact(
                x.ContactName,
                x.ContactTitle),
            new Address(
                x.Address,
                x.City,
                x.Region,
                x.PostalCode,
                x.Country),
            new CustomerCommunication(
                x.Phone,
                x.Fax,
                x.HomepageUrl));

    public static CustomerEntity ToCustomerEntity(this CustomerEntity entity, CustomerRequest request)
    {
        entity.CompanyName = request.CompanyName.Trim();
        entity.ContactName = request.Contact?.ContactName;
        entity.ContactTitle = request.Contact?.ContactTitle;
        entity.Address = request.Address?.AddressLine;
        entity.City = request.Address?.City;
        entity.Region = request.Address?.Region;
        entity.PostalCode = request.Address?.PostalCode;
        entity.Country = request.Address?.Country;
        entity.Phone = request.Communication?.Phone;
        entity.Fax = request.Communication?.Fax;
        entity.HomepageUrl = request.Communication?.HomepageUrl;

        return entity;
    }
}
