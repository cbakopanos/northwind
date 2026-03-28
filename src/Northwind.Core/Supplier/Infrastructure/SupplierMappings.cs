using System.Linq.Expressions;
using Northwind.Supplier.Application;

namespace Northwind.Supplier.Infrastructure;

public static class SupplierMappings
{
    public static readonly Expression<Func<SupplierEntity, SupplierSummaryDto>> ToSummaryDto =
        x => new SupplierSummaryDto(
            x.SupplierId,
            x.CompanyName,
            x.ContactName,
            x.ContactTitle);

    public static readonly Expression<Func<SupplierEntity, SupplierDetailsDto>> ToDetailsDto =
        x => new SupplierDetailsDto(
            x.SupplierId,
            x.CompanyName,
            x.ContactName,
            x.ContactTitle,
            x.Address,
            x.City,
            x.Region,
            x.PostalCode,
            x.Country,
            x.Phone,
            x.Fax,
            x.HomepageUrl);
}
