namespace Northwind.Catalog.Application;

public sealed record CategoryRequest(
    string CategoryName,
    string? Description);
