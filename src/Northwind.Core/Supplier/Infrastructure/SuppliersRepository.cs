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
            .Select(SupplierMappings.ToListItem)
            .ToListAsync(cancellationToken);

        logger.LogInformation("Fetched {SupplierCount} suppliers from database", suppliers.Count);

        return suppliers;
    }
}
