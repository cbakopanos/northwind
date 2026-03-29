using Northwind.Shared.Abstractions;

namespace Northwind.Supplier.Application;

public interface ISuppliersRepository
{
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    Task<PagedResult<SupplierSummaryDto>> GetAllAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);

    Task<SupplierDetailsDto?> GetByIdAsync(int supplierId, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(SupplierRequest request, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(int supplierId, SupplierRequest request, CancellationToken cancellationToken = default);
}
