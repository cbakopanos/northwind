using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Northwind.Supplier.Application;

namespace Northwind.Supplier.Infrastructure;

public sealed class SuppliersRepository(
    SupplierDbContext dbContext,
    ILogger<SuppliersRepository> logger) : ISuppliersRepository
{
    public async Task<IReadOnlyList<SupplierListItem>> Execute(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching suppliers from database");

        var suppliers = await dbContext.Suppliers
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

        logger.LogInformation("Fetched {SupplierCount} suppliers from database", suppliers.Count);

        return suppliers;
    }
}
