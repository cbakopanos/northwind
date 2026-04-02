namespace Northwind.Shared.Abstractions;

public interface IValidatable
{
    IReadOnlyList<string> Validate();
}
