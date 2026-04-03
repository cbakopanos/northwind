using Microsoft.Extensions.Logging;
using Northwind.Crm.Domain;
using Northwind.Shared.Abstractions;

namespace Northwind.Crm.Application;

public sealed class CustomersService(
    ICustomersRepository repository,
    ILogger<CustomersService> logger) : ICustomersService
{
    public Task<int> GetCountAsync(CancellationToken cancellationToken) => repository.GetCountAsync(cancellationToken);

    public async Task<PagedResult<CustomerSummaryDto>> GetAllAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        var customers = await repository.GetAllAsync(page, pageSize, cancellationToken);

        var items = customers.Items
            .Select(customer => customer.ToSummaryDto())
            .ToList();

        return new PagedResult<CustomerSummaryDto>(items, customers.Page, customers.PageSize, customers.TotalCount);
    }

    public async Task<CustomerDetailsDto?> GetByIdAsync(string customerId, CancellationToken cancellationToken)
    {
        var id = CustomerId.Create(customerId);
        var customer = await repository.GetByIdAsync(id, cancellationToken);
        return customer?.ToDetailsDto();
    }

    public async Task<string> CreateAsync(CustomerRequest request, CancellationToken cancellationToken)
    {
        const int maxAttempts = 20;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            var id = CustomerId.Generate();

            if (await repository.ExistsAsync(id, cancellationToken))
            {
                logger.LogWarning("Generated duplicate customer id {CustomerId}, retrying", id.Value);
                continue;
            }

            var customer = Customer.Create(
                id,
                request.CompanyName,
                request.Contact?.ContactName,
                request.Contact?.ContactTitle,
                request.Address?.AddressLine,
                request.Address?.City,
                request.Address?.Region,
                request.Address?.PostalCode,
                request.Address?.Country,
                request.Communication?.Phone,
                request.Communication?.Fax,
                request.Communication?.HomepageUrl);

            await repository.AddAsync(customer, cancellationToken);
            return customer.Id.Value;
        }

        throw new InvalidOperationException("Unable to generate a unique customer id after multiple attempts.");
    }

    public async Task<bool> UpdateAsync(string customerId, CustomerRequest request, CancellationToken cancellationToken)
    {
        var id = CustomerId.Create(customerId);
        var customer = await repository.GetByIdAsync(id, cancellationToken);

        if (customer is null)
            return false;

        customer.Update(
            request.CompanyName,
            request.Contact?.ContactName,
            request.Contact?.ContactTitle,
            request.Address?.AddressLine,
            request.Address?.City,
            request.Address?.Region,
            request.Address?.PostalCode,
            request.Address?.Country,
            request.Communication?.Phone,
            request.Communication?.Fax,
            request.Communication?.HomepageUrl);

        return await repository.UpdateAsync(customer, cancellationToken);
    }
}
