using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Northwind.Shared.Application;

namespace Northwind.Shared.Infrastructure;

public abstract class BaseRepository<TEntity>(
    DbContext dbContext,
    ILogger logger) : IBaseRepository
    where TEntity : class
{
    public async Task<int> GetCountAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching {EntityType} count from database", typeof(TEntity).Name);

        var count = await dbContext.Set<TEntity>()
            .CountAsync(cancellationToken);

        logger.LogInformation("{Count} {EntityType} found", count, typeof(TEntity).Name);

        return count;
    }
}
