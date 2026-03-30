namespace Northwind.Crm.Application;

public sealed record CustomerRequest(
    string CompanyName,
    CustomerContact? Contact,
    Address? Address,
    CustomerCommunication? Communication);