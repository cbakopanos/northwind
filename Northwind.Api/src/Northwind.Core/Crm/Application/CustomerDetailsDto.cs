namespace Northwind.Crm.Application;

public sealed record CustomerDetailsDto(
    string CustomerId,
    string CompanyName,
    CustomerContact Contact,
    Address Address,
    CustomerCommunication Communication);