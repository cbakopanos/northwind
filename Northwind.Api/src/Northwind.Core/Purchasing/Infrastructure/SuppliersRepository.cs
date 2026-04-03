using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Northwind.Shared.Abstractions;
using Northwind.Shared.Application;
using Northwind.Shared.Infrastructure;
using Northwind.Purchasing.Application;

namespace Northwind.Purchasing.Infrastructure;

public sealed class SuppliersRepository(
    SupplierDbContext dbContext,
    ILogger<SuppliersRepository> logger) : BaseRepository<SupplierEntity>(dbContext, logger), ISuppliersRepository
{
    public async Task<PagedResult<SupplierSummaryDto>> GetAllAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching suppliers from database (page {Page}, pageSize {PageSize})", page, pageSize);

        var totalCount = await dbContext.Suppliers.CountAsync(cancellationToken);

        var suppliers = await dbContext.Suppliers
            .AsNoTracking()
            .OrderBy(x => x.CreatedAt)            
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToSummaryDto)
            .ToListAsync(cancellationToken);

        logger.LogInformation("Fetched {SupplierCount} of {TotalCount} suppliers from database", suppliers.Count,
            totalCount);

        return new PagedResult<SupplierSummaryDto>(suppliers, page, pageSize, totalCount);
    }

    public async Task<SupplierDetailsDto?> GetByIdAsync(int supplierId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching supplier {SupplierId} from database", supplierId);

        var supplier = await dbContext.Suppliers
            .AsNoTracking()
            .Where(x => x.SupplierId == supplierId)
            .Select(ToDetailsDto)
            .SingleOrDefaultAsync(cancellationToken);

        logger.LogInformation(
            supplier is null
                ? "Supplier {SupplierId} was not found"
                : "Supplier {SupplierId} was found",
            supplierId);

        return supplier;
    }

    public async Task<int> CreateAsync(SupplierRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating supplier with CompanyName {CompanyName}", request.CompanyName);

        var entity = ToSupplierEntity(new SupplierEntity(), request);

        dbContext.Suppliers.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Created supplier with SupplierId {SupplierId}", entity.SupplierId);

        return entity.SupplierId;
    }

    public async Task<bool> UpdateAsync(int supplierId, SupplierRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating supplier {SupplierId}", supplierId);

        var entity = await dbContext.Suppliers
            .SingleOrDefaultAsync(x => x.SupplierId == supplierId, cancellationToken);

        if (entity is null)
        {
            logger.LogInformation("Supplier {SupplierId} was not found for update", supplierId);
            return false;
        }

        ToSupplierEntity(entity, request);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Updated supplier {SupplierId}", supplierId);
        return true;
    }
    static readonly Expression<Func<SupplierEntity, SupplierSummaryDto>> ToSummaryDto =
        x => new SupplierSummaryDto(
            x.SupplierId,
            x.CompanyName,
            new Contact(
                x.ContactName,
                x.ContactTitle));

    static readonly Expression<Func<SupplierEntity, SupplierDetailsDto>> ToDetailsDto =
        x => new SupplierDetailsDto(
            x.SupplierId,
            x.CompanyName,
            new Contact(
                x.ContactName,
                x.ContactTitle),
            new Address(
                x.Address,
                x.City,
                x.Region,
                x.PostalCode,
                x.Country),
            new Communication(
                x.Phone,
                x.Fax,
                x.HomepageUrl));

    static SupplierEntity ToSupplierEntity(SupplierEntity entity, SupplierRequest request)
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

}