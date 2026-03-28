using Microsoft.EntityFrameworkCore;
using Northwind.Supplier.Application;

namespace Northwind.Supplier.Infrastructure;

public sealed class GetAllSuppliersQuery(SupplierDbContext dbContext) : IGetAllSuppliers
{
    public async Task<IReadOnlyList<SupplierListItem>> Execute(CancellationToken cancellationToken = default)
    {
        return await dbContext.Suppliers
            .AsNoTracking()
            .OrderBy(x => x.CompanyName)
            .Select(x => new SupplierListItem(
                x.SupplierId,
                x.CompanyName,
                x.ContactName,
                x.ContactTitle,
                x.City,
                x.Country,
                x.Phone))
            .ToListAsync(cancellationToken);
    }
}
