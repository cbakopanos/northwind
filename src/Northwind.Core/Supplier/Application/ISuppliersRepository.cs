namespace Northwind.Supplier.Application;

public interface ISuppliersRepository
{
    Task<IReadOnlyList<SupplierListItem>> Execute(CancellationToken cancellationToken = default);
}
