using Northwind.Shared.Abstractions;
using Northwind.Shared.Application;

namespace Northwind.Crm.Application;

public interface ICustomersRepository : IBaseRepository
{
    Task<PagedResult<CustomerSummaryDto>> GetAllAsync(int page = 1, int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<CustomerDetailsDto?> GetByIdAsync(string customerId, CancellationToken cancellationToken = default);

    Task<string> CreateAsync(CustomerRequest request, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(string customerId, CustomerRequest request, CancellationToken cancellationToken = default);
}