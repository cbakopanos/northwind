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

    public async Task<int> CreateAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating supplier with CompanyName {CompanyName}", request.CompanyName);

        var entity = CreateEntity(request);

        dbContext.Suppliers.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Created supplier with SupplierId {SupplierId}", entity.SupplierId);

        return entity.SupplierId;
    }

    private static SupplierEntity CreateEntity(CreateSupplierRequest request) =>
        new()
        {
            CompanyName = request.CompanyName.Trim(),
            ContactName = request.Contact?.ContactName,
            ContactTitle = request.Contact?.ContactTitle,
            Address = request.Address?.AddressLine,
            City = request.Address?.City,
            Region = request.Address?.Region,
            PostalCode = request.Address?.PostalCode,
            Country = request.Address?.Country,
            Phone = request.Communication?.Phone,
            Fax = request.Communication?.Fax,
            HomepageUrl = request.Communication?.HomepageUrl
        };
}
