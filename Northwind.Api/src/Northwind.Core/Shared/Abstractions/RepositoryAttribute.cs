namespace Northwind.Shared.Abstractions;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class RepositoryAttribute(Type serviceType, Type implementationType) : Attribute
{
    public Type ServiceType { get; } = ValidateServiceType(serviceType);

    public Type ImplementationType { get; } = ValidateImplementationType(serviceType, implementationType);

    public bool IsValidRepositoryRegistration =>
        ServiceType.IsAssignableFrom(ImplementationType)
        && ImplementationType.IsClass
        && !ImplementationType.IsAbstract
        && ImplementationType.IsSealed;

    private static Type ValidateServiceType(Type serviceType)
    {
        ArgumentNullException.ThrowIfNull(serviceType);

        return serviceType;
    }

    private static Type ValidateImplementationType(Type serviceType, Type implementationType)
    {
        ArgumentNullException.ThrowIfNull(implementationType);

        if (!implementationType.IsClass)
        {
            throw new ArgumentException(
                $"Repository implementation type '{implementationType.FullName}' must be a class.",
                nameof(implementationType));
        }

        if (implementationType.IsAbstract)
        {
            throw new ArgumentException(
                $"Repository implementation type '{implementationType.FullName}' must not be abstract.",
                nameof(implementationType));
        }

        if (!implementationType.IsSealed)
        {
            throw new ArgumentException(
                $"Repository implementation type '{implementationType.FullName}' must be sealed.",
                nameof(implementationType));
        }

        if (!serviceType.IsAssignableFrom(implementationType))
        {
            throw new ArgumentException(
                $"Repository implementation type '{implementationType.FullName}' must implement/derive from '{serviceType.FullName}'.",
                nameof(implementationType));
        }

        return implementationType;
    }
}
