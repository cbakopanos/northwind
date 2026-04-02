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
        return errors;
    }
}

