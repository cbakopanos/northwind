namespace Northwind.Shared.Application;

public sealed record Communication(
    string? Phone,
    string? Fax,
    string? HomepageUrl)
{
    public IEnumerable<string> Validate()
    {
        if (Phone?.Length > 24)
            yield return "Phone cannot exceed 24 characters.";
        if (Fax?.Length > 24)
            yield return "Fax cannot exceed 24 characters.";
    }
}

public sealed record Contact(
    string? ContactName,
    string? ContactTitle)
{
    public IEnumerable<string> Validate()
    {
        if (ContactName?.Length > 30)
            yield return "ContactName cannot exceed 30 characters.";
        if (ContactTitle?.Length > 30)
            yield return "ContactTitle cannot exceed 30 characters.";
    }
}

public sealed record Address(
    string? AddressLine,
    string? City,
    string? Region,
    string? PostalCode,
    string? Country)
{
    public IEnumerable<string> Validate()
    {
        if (AddressLine?.Length > 60)
            yield return "Address cannot exceed 60 characters.";
        if (City?.Length > 15)
            yield return "City cannot exceed 15 characters.";
        if (Region?.Length > 15)
            yield return "Region cannot exceed 15 characters.";
        if (PostalCode?.Length > 10)
            yield return "PostalCode cannot exceed 10 characters.";
        if (Country?.Length > 15)
            yield return "Country cannot exceed 15 characters.";
    }
}
    