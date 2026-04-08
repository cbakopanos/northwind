using Northwind.Purchasing.Domain;
using Northwind.Shared.Abstractions;

namespace Northwind.Purchasing.Application;

public sealed class SuppliersService(
    ISuppliersRepository repository) : ISuppliersService
{
    public Task<int> GetCountAsync(CancellationToken cancellationToken) => repository.GetCountAsync(cancellationToken);

    public async Task<PagedResult<SupplierSummaryDto>> GetAllAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        var suppliers = await repository.GetAllAsync(page, pageSize, cancellationToken);

        var items = suppliers.Items
            .Select(supplier => supplier.ToSummaryDto())
            .ToList();

        return new PagedResult<SupplierSummaryDto>(items, suppliers.Page, suppliers.PageSize, suppliers.TotalCount);
    }

    public async Task<SupplierDetailsDto?> GetByIdAsync(int supplierId, CancellationToken cancellationToken)
    {
        var id = SupplierId.Create(supplierId);
        var supplier = await repository.GetByIdAsync(id, cancellationToken);
        return supplier?.ToDetailsDto();
    }

    public async Task<int> CreateAsync(SupplierRequest request, CancellationToken cancellationToken)
    {
        var supplier = Supplier.Create(
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

        var id = await repository.AddAsync(supplier, cancellationToken);
        return id.Value;
    }

    public async Task<bool> UpdateAsync(int supplierId, SupplierRequest request, CancellationToken cancellationToken)
    {
        var id = SupplierId.Create(supplierId);
        var supplier = await repository.GetByIdAsync(id, cancellationToken);

        if (supplier is null)
            return false;

        supplier.Update(
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

        return await repository.UpdateAsync(supplier, cancellationToken);
    }
}
