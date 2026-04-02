namespace Northwind.Shared.Application;

public interface IBaseRepository
{
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
}
