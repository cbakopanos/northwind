using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Northwind.Shared.Abstractions;
using Northwind.Shared.Infrastructure;
using Northwind.Crm.Application;
using System.Linq.Expressions;
using Northwind.Shared.Application;

namespace Northwind.Crm.Infrastructure;

public sealed class CustomersRepository(
    CrmDbContext dbContext,
    ILogger<CustomersRepository> logger) : BaseRepository<CustomerEntity>(dbContext, logger), ICustomersRepository
{
    public async Task<PagedResult<CustomerSummaryDto>> GetAllAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching customers from database (page {Page}, pageSize {PageSize})", page, pageSize);

        var totalCount = await dbContext.Customers.CountAsync(cancellationToken);

        var customers = await dbContext.Customers
            .AsNoTracking()
            .OrderByDescending(x => x.CustomerId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToSummaryDto)
            .ToListAsync(cancellationToken);

        logger.LogInformation("Fetched {CustomerCount} of {TotalCount} customers from database", customers.Count,
            totalCount);

        return new PagedResult<CustomerSummaryDto>(customers, page, pageSize, totalCount);
    }

    public async Task<CustomerDetailsDto?> GetByIdAsync(string customerId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching customer {CustomerId} from database", customerId);

        var customer = await dbContext.Customers
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .Select(ToDetailsDto)
            .SingleOrDefaultAsync(cancellationToken);

        logger.LogInformation(
            customer is null
                ? "Customer {CustomerId} was not found"
                : "Customer {CustomerId} was found",
            customerId);

        return customer;
    }

    public async Task<string> CreateAsync(CustomerRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating customer with CompanyName {CompanyName}", request.CompanyName);

        // Generate a simple 5-char ID (uppercase alphanumeric) and ensure uniqueness
        string genId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var rng = new Random();
            return new string(Enumerable.Range(0, 5).Select(_ => chars[rng.Next(chars.Length)]).ToArray());
        }

        string customerId;
        CustomerEntity entity;

        do
        {
            customerId = genId();
            entity = new CustomerEntity { CustomerId = customerId };
            ToCustomerEntity(entity, request);
            dbContext.Customers.Add(entity);
            try
            {
                await dbContext.SaveChangesAsync(cancellationToken);
                break;
            }
            catch (DbUpdateException)
            {
                // assume conflict -> try again
                dbContext.Entry(entity).State = EntityState.Detached;
            }
        } while (true);

        logger.LogInformation("Created customer with CustomerId {CustomerId}", customerId);

        return customerId;
    }

    public async Task<bool> UpdateAsync(string customerId, CustomerRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating customer {CustomerId}", customerId);

        var entity = await dbContext.Customers
            .SingleOrDefaultAsync(x => x.CustomerId == customerId, cancellationToken);

        if (entity is null)
        {
            logger.LogInformation("Customer {CustomerId} was not found for update", customerId);
            return false;
        }

        ToCustomerEntity(entity, request);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Updated customer {CustomerId}", customerId);
        return true;
    }

    static readonly Expression<Func<CustomerEntity, CustomerSummaryDto>> ToSummaryDto =
        x => new CustomerSummaryDto(
            x.CustomerId,
            x.CompanyName,
            new Contact(
                x.ContactName,
                x.ContactTitle));

    static readonly Expression<Func<CustomerEntity, CustomerDetailsDto>> ToDetailsDto =
        x => new CustomerDetailsDto(
            x.CustomerId,
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

    static CustomerEntity ToCustomerEntity(CustomerEntity entity, CustomerRequest request)
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
