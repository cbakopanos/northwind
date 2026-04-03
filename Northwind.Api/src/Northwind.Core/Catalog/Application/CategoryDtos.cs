using Northwind.Shared.Abstractions;

namespace Northwind.Catalog.Application;

public sealed record CategoryDetailsDto(
    int CategoryId,
    string CategoryName,
    string? Description,
    bool HasPicture);

public sealed record CategorySummaryDto(
    int CategoryId,
    string CategoryName,
    bool HasPicture);

public sealed record CategoryRequest(
    string CategoryName,
    string? Description) : IValidatable
{
    public IReadOnlyList<string> Validate()
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(CategoryName))
            errors.Add("CategoryName is required.");
        else if (CategoryName.Length > 15)
            errors.Add("CategoryName cannot exceed 15 characters.");
        return errors;
    }
}

