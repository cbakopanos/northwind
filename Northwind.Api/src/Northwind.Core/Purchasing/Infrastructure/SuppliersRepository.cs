using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Northwind.Purchasing.Domain;
using Northwind.Shared.Abstractions;
using Northwind.Shared.Infrastructure;

namespace Northwind.Purchasing.Infrastructure;

public sealed class SuppliersRepository(
    SupplierDbContext dbContext,
    ILogger<SuppliersRepository> logger) : BaseRepository<SupplierEntity>(dbContext, logger), ISuppliersRepository
{
    public async Task<PagedResult<Supplier>> GetAllAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching suppliers from database (page {Page}, pageSize {PageSize})", page, pageSize);

        var totalCount = await dbContext.Suppliers.CountAsync(cancellationToken);

        var suppliers = await dbContext.Suppliers
            .AsNoTracking()
            .OrderBy(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var domainSuppliers = suppliers
            .Select(ToDomain)
            .ToList();

        logger.LogInformation("Fetched {SupplierCount} of {TotalCount} suppliers from database", suppliers.Count,
            totalCount);

        return new PagedResult<Supplier>(domainSuppliers, page, pageSize, totalCount);
    }

    public async Task<Supplier?> GetByIdAsync(SupplierId supplierId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching supplier {SupplierId} from database", supplierId.Value);

        var entity = await dbContext.Suppliers
            .AsNoTracking()
            .Where(x => x.SupplierId == supplierId.Value)
            .SingleOrDefaultAsync(cancellationToken);

        if (entity is null)
        {
            logger.LogInformation("Supplier {SupplierId} was not found", supplierId.Value);
            return null;
        }

        logger.LogInformation("Supplier {SupplierId} was found", supplierId.Value);
        return ToDomain(entity);
    }

    public async Task<SupplierId> AddAsync(Supplier supplier, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating supplier with CompanyName {CompanyName}", supplier.CompanyName.Value);

        var entity = ToEntity(supplier, new SupplierEntity());

        dbContext.Suppliers.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        var id = SupplierId.FromPersistence(entity.SupplierId);
        supplier.AssignId(id);

        logger.LogInformation("Created supplier with SupplierId {SupplierId}", entity.SupplierId);

        return id;
    }

    public async Task<bool> UpdateAsync(Supplier supplier,
        CancellationToken cancellationToken)
    {
        var supplierId = supplier.Id?.Value ?? throw new InvalidOperationException("Supplier id is not assigned.");

        logger.LogInformation("Updating supplier {SupplierId}", supplierId);

        var entity = await dbContext.Suppliers
            .SingleOrDefaultAsync(x => x.SupplierId == supplierId, cancellationToken);

        if (entity is null)
        {
            logger.LogInformation("Supplier {SupplierId} was not found for update", supplierId);
            return false;
        }

        ToEntity(supplier, entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Updated supplier {SupplierId}", supplierId);
        return true;
    }

    static Supplier ToDomain(SupplierEntity entity) =>
        Supplier.Rehydrate(
            entity.SupplierId,
            entity.CompanyName,
            entity.ContactName,
            entity.ContactTitle,
            entity.Address,
            entity.City,
            entity.Region,
            entity.PostalCode,
            entity.Country,
            entity.Phone,
            entity.Fax,
            entity.HomepageUrl);

    static SupplierEntity ToEntity(Supplier supplier, SupplierEntity entity)
    {
        entity.CompanyName = supplier.CompanyName.Value;
        entity.ContactName = supplier.Contact.ContactName;
        entity.ContactTitle = supplier.Contact.ContactTitle;
        entity.Address = supplier.Address.AddressLine;
        entity.City = supplier.Address.City;
        entity.Region = supplier.Address.Region;
        entity.PostalCode = supplier.Address.PostalCode;
        entity.Country = supplier.Address.Country;
        entity.Phone = supplier.Communication.Phone;
        entity.Fax = supplier.Communication.Fax;
        entity.HomepageUrl = supplier.Communication.HomepageUrl;

        return entity;
    }
}