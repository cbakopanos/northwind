using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Northwind.Shared.Abstractions;
using Northwind.Crm.Application;

namespace Northwind.Crm.Infrastructure;

public sealed class CustomersRepository(
    CrmDbContext dbContext,
    ILogger<CustomersRepository> logger) : ICustomersRepository
{
    public async Task<PagedResult<CustomerSummaryDto>> GetAllAsync(int page = 1, int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching customers from database (page {Page}, pageSize {PageSize})", page, pageSize);

        var totalCount = await dbContext.Customers.CountAsync(cancellationToken);

        var customers = await dbContext.Customers
            .AsNoTracking()
            .OrderBy(x => x.CompanyName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(CustomerMappings.ToSummaryDto)
            .ToListAsync(cancellationToken);

        logger.LogInformation("Fetched {CustomerCount} of {TotalCount} customers from database", customers.Count,
            totalCount);

        return new PagedResult<CustomerSummaryDto>(customers, page, pageSize, totalCount);
    }

    public async Task<CustomerDetailsDto?> GetByIdAsync(string customerId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching customer {CustomerId} from database", customerId);

        var customer = await dbContext.Customers
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .Select(CustomerMappings.ToDetailsDto)
            .SingleOrDefaultAsync(cancellationToken);

        logger.LogInformation(
            customer is null
                ? "Customer {CustomerId} was not found"
                : "Customer {CustomerId} was found",
            customerId);

        return customer;
    }

    public async Task<string> CreateAsync(CustomerRequest request, CancellationToken cancellationToken = default)
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
            entity.ToCustomerEntity(request);
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
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating customer {CustomerId}", customerId);

        var entity = await dbContext.Customers
            .SingleOrDefaultAsync(x => x.CustomerId == customerId, cancellationToken);

        if (entity is null)
        {
            logger.LogInformation("Customer {CustomerId} was not found for update", customerId);
            return false;
        }

        entity.ToCustomerEntity(request);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Updated customer {CustomerId}", customerId);
        return true;
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching customer count from database");

        var count = await dbContext.Customers
            .CountAsync(cancellationToken);

        logger.LogInformation("{CustomerCount} customers found", count);

        return count;
    }
}
