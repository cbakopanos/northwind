using System.Linq.Expressions;
using Northwind.Supplier.Application;

namespace Northwind.Supplier.Infrastructure;

public static class SupplierMappings
{
    public static SupplierEntity ToSupplierEntity(this SupplierEntity entity, SupplierRequest request)
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

    public static readonly Expression<Func<SupplierEntity, SupplierSummaryDto>> ToSummaryDto =
        x => new SupplierSummaryDto(
            x.SupplierId,
            x.CompanyName,
            new SupplierContact(
                x.ContactName,
                x.ContactTitle));

    public static readonly Expression<Func<SupplierEntity, SupplierDetailsDto>> ToDetailsDto =
        x => new SupplierDetailsDto(
            x.SupplierId,
            x.CompanyName,
            new SupplierContact(
                x.ContactName,
                x.ContactTitle),
            new Address(
                x.Address,
                x.City,
                x.Region,
                x.PostalCode,
                x.Country),
            new SupplierCommunication(
                x.Phone,
                x.Fax,
                x.HomepageUrl));
}
