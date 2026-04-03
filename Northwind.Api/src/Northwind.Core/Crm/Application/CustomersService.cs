using Northwind.Crm.Domain;
using Northwind.Shared.Abstractions;

namespace Northwind.Crm.Application;

public sealed class CustomersService(
    ICustomersRepository repository) : ICustomersService
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
        var customer = Customer.Create(
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

        var id = await repository.AddAsync(customer, cancellationToken);
        return id.Value;
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
