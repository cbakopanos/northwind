using Northwind.Shared.Abstractions;

namespace Northwind.Purchasing.Domain;

public interface ISuppliersRepository
{
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    Task<PagedResult<Supplier>> GetAllAsync(int page = 1, int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<Supplier?> GetByIdAsync(SupplierId supplierId, CancellationToken cancellationToken = default);

    Task<SupplierId> AddAsync(Supplier supplier, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Supplier supplier, CancellationToken cancellationToken = default);
}
