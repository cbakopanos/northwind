namespace Northwind.Shared.Application;

public sealed record Communication(
    string? Phone,
    string? Fax,
    string? HomepageUrl);

public sealed record Contact(
    string? ContactName,
    string? ContactTitle);

public sealed record Address(
    string? AddressLine,
    string? City,
    string? Region,
    string? PostalCode,
    string? Country);

public static class SharedValidation
{
    public static IEnumerable<string> Validate(this Contact? contact)
    {
        if (contact is null) yield break;
        if (contact.ContactName?.Length > 30)
            yield return "ContactName cannot exceed 30 characters.";
        if (contact.ContactTitle?.Length > 30)
            yield return "ContactTitle cannot exceed 30 characters.";
    }

    public static IEnumerable<string> Validate(this Address? address)
    {
        if (address is null) yield break;
        if (address.AddressLine?.Length > 60)
            yield return "Address cannot exceed 60 characters.";
        if (address.City?.Length > 15)
            yield return "City cannot exceed 15 characters.";
        if (address.Region?.Length > 15)
            yield return "Region cannot exceed 15 characters.";
        if (address.PostalCode?.Length > 10)
            yield return "PostalCode cannot exceed 10 characters.";
        if (address.Country?.Length > 15)
            yield return "Country cannot exceed 15 characters.";
    }

    public static IEnumerable<string> Validate(this Communication? communication)
    {
        if (communication is null) yield break;
        if (communication.Phone?.Length > 24)
            yield return "Phone cannot exceed 24 characters.";
        if (communication.Fax?.Length > 24)
            yield return "Fax cannot exceed 24 characters.";
    }
}
    