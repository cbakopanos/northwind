using Northwind.Shared.Abstractions;
using Northwind.Shared.Application;

namespace Northwind.Purchasing.Application;

public interface ISuppliersRepository : IBaseRepository
{
    Task<PagedResult<SupplierSummaryDto>> GetAllAsync(int page = 1, int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<SupplierDetailsDto?> GetByIdAsync(int supplierId, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(SupplierRequest request, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(int supplierId, SupplierRequest request, CancellationToken cancellationToken = default);
}