using Microsoft.EntityFrameworkCore;

namespace Northwind.Shared.Abstractions;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class InfrastructureAttribute(Type dbContextType, string connectionStringName = "Northwind") : Attribute
{
    public Type DbContextType { get; } = ValidateDbContextType(dbContextType);

    public string ConnectionStringName { get; } = connectionStringName;

    public bool IsValidDbContextType =>
        DbContextType.IsClass
        && !DbContextType.IsAbstract
        && DbContextType.IsSealed
        && typeof(DbContext).IsAssignableFrom(DbContextType);

    private static Type ValidateDbContextType(Type dbContextType)
    {
        ArgumentNullException.ThrowIfNull(dbContextType);

        if (!dbContextType.IsClass)
        {
            throw new ArgumentException(
                $"Infrastructure DbContext type '{dbContextType.FullName}' must be a class.",
                nameof(dbContextType));
        }

        if (dbContextType.IsAbstract)
        {
            throw new ArgumentException(
                $"Infrastructure DbContext type '{dbContextType.FullName}' must not be abstract.",
                nameof(dbContextType));
        }

        if (!typeof(DbContext).IsAssignableFrom(dbContextType))
        {
            throw new ArgumentException(
                $"Infrastructure DbContext type '{dbContextType.FullName}' must derive from '{typeof(DbContext).FullName}'.",
                nameof(dbContextType));
        }

        if (!dbContextType.IsSealed)
        {
            throw new ArgumentException(
                $"Infrastructure DbContext type '{dbContextType.FullName}' must be sealed.",
                nameof(dbContextType));
        }

        return dbContextType;
    }
}
