using Microsoft.EntityFrameworkCore;

namespace Northwind.Supplier.Infrastructure;

public sealed class SupplierDbContext(DbContextOptions<SupplierDbContext> options) : DbContext(options)
{
    public DbSet<SupplierEntity> Suppliers => Set<SupplierEntity>();
}
