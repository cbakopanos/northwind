namespace Northwind.Supplier.Application;

public interface IGetAllSuppliers
{
    Task<IReadOnlyList<SupplierListItem>> Execute(CancellationToken cancellationToken = default);
}
