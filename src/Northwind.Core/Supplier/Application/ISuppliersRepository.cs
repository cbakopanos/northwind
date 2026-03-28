namespace Northwind.Supplier.Application;

public interface ISuppliersRepository
{
    Task<IReadOnlyList<SupplierSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<SupplierDetailsDto?> GetByIdAsync(int supplierId, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default);
}
