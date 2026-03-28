namespace Northwind.Supplier.Application;

public interface IGetAllSuppliers
{
    Task<IReadOnlyList<SupplierListItem>> Execute(CancellationToken cancellationToken = default);
}

public sealed record SupplierListItem(
    int SupplierId,
    string CompanyName,
    string? ContactName,
    string? ContactTitle,
    string? City,
    string? Country,
    string? Phone);
