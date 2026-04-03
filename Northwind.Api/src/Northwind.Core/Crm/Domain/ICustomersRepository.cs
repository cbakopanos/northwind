using Northwind.Shared.Abstractions;

namespace Northwind.Crm.Domain;

public interface ICustomersRepository
{
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    Task<PagedResult<Customer>> GetAllAsync(int page = 1, int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<Customer?> GetByIdAsync(CustomerId customerId, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(CustomerId customerId, CancellationToken cancellationToken = default);

    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Customer customer, CancellationToken cancellationToken = default);
}
