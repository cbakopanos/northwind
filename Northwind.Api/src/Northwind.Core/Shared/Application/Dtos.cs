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
    