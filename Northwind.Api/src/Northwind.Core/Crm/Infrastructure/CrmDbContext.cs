using Microsoft.EntityFrameworkCore;

namespace Northwind.Crm.Infrastructure;

public sealed class CrmDbContext(DbContextOptions<CrmDbContext> options) : DbContext(options)
{
    public DbSet<CustomerEntity> Customers => Set<CustomerEntity>();
}
