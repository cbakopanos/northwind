namespace Northwind.Crm.Application;

public sealed record Address(
    string? AddressLine,
    string? City,
    string? Region,
    string? PostalCode,
    string? Country);