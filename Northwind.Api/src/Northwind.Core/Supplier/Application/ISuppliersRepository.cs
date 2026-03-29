namespace Northwind.Supplier.Application;

public interface ISuppliersRepository
{
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SupplierSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<SupplierDetailsDto?> GetByIdAsync(int supplierId, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(SupplierRequest request, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(int supplierId, SupplierRequest request, CancellationToken cancellationToken = default);
}
