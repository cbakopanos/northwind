namespace Northwind.Catalog.Application;

public sealed record CategoryDetailsDto(
    int CategoryId,
    string CategoryName,
    string? Description,
    bool HasPicture);