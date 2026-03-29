namespace Northwind.Catalog.Application;

public sealed record InventoryLevel(
    short UnitsInStock,
    short UnitsOnOrder);
