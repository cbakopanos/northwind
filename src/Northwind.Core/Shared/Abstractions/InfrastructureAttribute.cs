using Microsoft.EntityFrameworkCore;

namespace Northwind.Shared.Abstractions;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class InfrastructureAttribute(Type dbContextType, string connectionStringName = "Northwind") : Attribute
{
    public Type DbContextType { get; } = dbContextType;

    public string ConnectionStringName { get; } = connectionStringName;

    public bool IsValidDbContextType => typeof(DbContext).IsAssignableFrom(DbContextType);
}
