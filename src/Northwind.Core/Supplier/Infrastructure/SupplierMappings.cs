using System.Linq.Expressions;
using Northwind.Supplier.Application;

namespace Northwind.Supplier.Infrastructure;

public static class SupplierMappings
{
    public static readonly Expression<Func<SupplierEntity, SupplierListItem>> ToListItem =
        x => new SupplierListItem(
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
