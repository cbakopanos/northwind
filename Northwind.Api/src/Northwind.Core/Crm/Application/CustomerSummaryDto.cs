namespace Northwind.Crm.Application;

public sealed record CustomerSummaryDto(
    string CustomerId,
    string CompanyName,
    CustomerContact Contact);