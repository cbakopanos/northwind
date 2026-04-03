using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Northwind.Shared.Infrastructure;
using Northwind.Crm.Domain;
using Northwind.Shared.Abstractions;

namespace Northwind.Crm.Infrastructure;

public sealed class CustomersRepository(
    CrmDbContext dbContext,
    ILogger<CustomersRepository> logger) : BaseRepository<CustomerEntity>(dbContext, logger), ICustomersRepository
{
    public async Task<PagedResult<Customer>> GetAllAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching customers from database (page {Page}, pageSize {PageSize})", page, pageSize);

        var totalCount = await dbContext.Customers.CountAsync(cancellationToken);

        var customers = await dbContext.Customers
            .AsNoTracking()
            .OrderBy(x => x.CreatedAt)            
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var domainCustomers = customers
            .Select(ToDomain)
            .ToList();

        logger.LogInformation("Fetched {CustomerCount} of {TotalCount} customers from database", customers.Count,
            totalCount);

        return new PagedResult<Customer>(domainCustomers, page, pageSize, totalCount);
    }

    public async Task<Customer?> GetByIdAsync(CustomerId customerId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching customer {CustomerId} from database", customerId.Value);

        var customer = await dbContext.Customers
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId.Value)
            .SingleOrDefaultAsync(cancellationToken);

        logger.LogInformation(
            customer is null
                ? "Customer {CustomerId} was not found"
                : "Customer {CustomerId} was found",
            customerId.Value);

        return customer is null ? null : ToDomain(customer);
    }

    public async Task<CustomerId> AddAsync(Customer customer, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating customer");

        var entity = ToEntity(customer, new CustomerEntity());
        dbContext.Customers.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(entity.CustomerId))
            throw new InvalidOperationException("Database did not generate customer id.");

        var id = CustomerId.FromPersistence(entity.CustomerId);
        customer.AssignId(id);

        logger.LogInformation("Created customer with CustomerId {CustomerId}", id.Value);
        return id;
    }

    public async Task<bool> UpdateAsync(Customer customer,
        CancellationToken cancellationToken)
    {
        var customerId = customer.Id?.Value ?? throw new InvalidOperationException("Customer id is not assigned.");

        logger.LogInformation("Updating customer {CustomerId}", customerId);

        var entity = await dbContext.Customers
            .SingleOrDefaultAsync(x => x.CustomerId == customerId, cancellationToken);

        if (entity is null)
        {
            logger.LogInformation("Customer {CustomerId} was not found for update", customerId);
            return false;
        }

        ToEntity(customer, entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Updated customer {CustomerId}", customerId);
        return true;
    }

    static Customer ToDomain(CustomerEntity entity)
    {
        if (string.IsNullOrWhiteSpace(entity.CustomerId))
            throw new InvalidOperationException("Customer row has null or empty customer_id.");

        return Customer.Rehydrate(
            entity.CustomerId,
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
    }

    static CustomerEntity ToEntity(Customer customer, CustomerEntity entity)
    {
        entity.CompanyName = customer.CompanyName.Value;
        entity.ContactName = customer.Contact.ContactName;
        entity.ContactTitle = customer.Contact.ContactTitle;
        entity.Address = customer.Address.AddressLine;
        entity.City = customer.Address.City;
        entity.Region = customer.Address.Region;
        entity.PostalCode = customer.Address.PostalCode;
        entity.Country = customer.Address.Country;
        entity.Phone = customer.Communication.Phone;
        entity.Fax = customer.Communication.Fax;
        entity.HomepageUrl = customer.Communication.HomepageUrl;

        return entity;
    }

}
