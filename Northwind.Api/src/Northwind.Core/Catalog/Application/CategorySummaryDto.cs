namespace Northwind.Catalog.Application;

public sealed record CategorySummaryDto(
    int CategoryId,
    string CategoryName,
    bool HasPicture);
