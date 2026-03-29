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

    public async Task<int> CreateAsync(SupplierRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating supplier with CompanyName {CompanyName}", request.CompanyName);

        var entity = new SupplierEntity().ToSupplierEntity(request);

        dbContext.Suppliers.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Created supplier with SupplierId {SupplierId}", entity.SupplierId);

        return entity.SupplierId;
    }

    public async Task<bool> UpdateAsync(int supplierId, SupplierRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating supplier {SupplierId}", supplierId);

        var entity = await dbContext.Suppliers
            .SingleOrDefaultAsync(x => x.SupplierId == supplierId, cancellationToken);

        if (entity is null)
        {
            logger.LogInformation("Supplier {SupplierId} was not found for update", supplierId);
            return false;
        }

        entity.ToSupplierEntity(request);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Updated supplier {SupplierId}", supplierId);
        return true;
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching supplier count from database");

        var count = await dbContext.Suppliers
            .CountAsync(cancellationToken);

        logger.LogInformation("{SupplierCount} suppliers found", count);

        return count;
    }
}
