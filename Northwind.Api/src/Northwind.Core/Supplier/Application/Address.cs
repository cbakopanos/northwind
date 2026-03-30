namespace Northwind.Supplier.Application;

public sealed record Address(
    string? AddressLine,
    string? City,
    string? Region,
    string? PostalCode,
    string? Country);