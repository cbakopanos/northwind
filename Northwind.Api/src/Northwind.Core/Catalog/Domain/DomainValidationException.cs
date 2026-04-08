namespace Northwind.Catalog.Domain;

public sealed class DomainValidationException(IReadOnlyList<string> errors)
    : Exception("Domain validation failed.")
{
    public IReadOnlyList<string> Errors { get; } = errors;
}
