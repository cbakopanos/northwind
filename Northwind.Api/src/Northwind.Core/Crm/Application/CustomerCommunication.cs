namespace Northwind.Crm.Application;

public sealed record CustomerCommunication(
    string? Phone,
    string? Fax,
    string? HomepageUrl);