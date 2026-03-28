using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Northwind.Supplier.Application;

namespace Northwind.Supplier.Infrastructure;

public sealed class SuppliersRepository(
    SupplierDbContext dbContext,
    ILogger<SuppliersRepository> logger) : ISuppliersRepository
{
    public async Task<IReadOnlyList<SupplierSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching suppliers from database");

        var suppliers = await dbContext.Suppliers
            .AsNoTracking()
            .OrderBy(x => x.CompanyName)
            .Select(SupplierMappings.ToSummaryDto)
            .ToListAsync(cancellationToken);

        logger.LogInformation("Fetched {SupplierCount} suppliers from database", suppliers.Count);

        return suppliers;
    }

    public async Task<SupplierDetailsDto?> GetByIdAsync(int supplierId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching supplier {SupplierId} from database", supplierId);

        var supplier = await dbContext.Suppliers
            .AsNoTracking()
            .Where(x => x.SupplierId == supplierId)
            .Select(SupplierMappings.ToDetailsDto)
            .SingleOrDefaultAsync(cancellationToken);

        logger.LogInformation(
            supplier is null
                ? "Supplier {SupplierId} was not found"
                : "Supplier {SupplierId} was found",
            supplierId);

        return supplier;
    }
}
