using Northwind.Shared.Abstractions;
using Northwind.Shared.Application;

namespace Northwind.Crm.Application;

public sealed record CustomerDetailsDto(
    string CustomerId,
    string CompanyName,
    Contact Contact,
    Address Address,
    Communication Communication);

public sealed record CustomerSummaryDto(
    string CustomerId,
    string CompanyName,
    Contact Contact);

public sealed record CustomerRequest(
    string CompanyName,
    Contact? Contact,
    Address? Address,
    Communication? Communication) : IValidatable
{
    public IReadOnlyList<string> Validate()
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(CompanyName))
            errors.Add("CompanyName is required.");
        else if (CompanyName.Length > 40)
            errors.Add("CompanyName cannot exceed 40 characters.");
        if (Contact is not null)
            errors.AddRange(Contact.Validate());
        if (Address is not null)
            errors.AddRange(Address.Validate());
        if (Communication is not null)
            errors.AddRange(Communication.Validate());
        return errors;
    }
}

