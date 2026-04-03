namespace Northwind.Crm.Domain;

public sealed class Customer
{
    Customer(
        CustomerId? id,
        CompanyName companyName,
        CustomerContact contact,
        CustomerAddress address,
        CustomerCommunication communication)
    {
        Id = id;
        CompanyName = companyName;
        Contact = contact;
        Address = address;
        Communication = communication;
    }

    public CustomerId? Id { get; private set; }
    public CompanyName CompanyName { get; private set; }
    public CustomerContact Contact { get; private set; }
    public CustomerAddress Address { get; private set; }
    public CustomerCommunication Communication { get; private set; }

    public static Customer Create(
        string companyName,
        string? contactName,
        string? contactTitle,
        string? addressLine,
        string? city,
        string? region,
        string? postalCode,
        string? country,
        string? phone,
        string? fax,
        string? homepageUrl)
    {
        var errors = Validate(companyName, contactName, contactTitle, addressLine, city, region, postalCode, country,
            phone, fax);

        if (errors.Count > 0)
            throw new DomainValidationException(errors);

        return new Customer(
            null,
            CompanyName.Create(companyName),
            CustomerContact.Create(contactName, contactTitle),
            CustomerAddress.Create(addressLine, city, region, postalCode, country),
            CustomerCommunication.Create(phone, fax, homepageUrl));
    }

    public static Customer Rehydrate(
        string customerId,
        string companyName,
        string? contactName,
        string? contactTitle,
        string? addressLine,
        string? city,
        string? region,
        string? postalCode,
        string? country,
        string? phone,
        string? fax,
        string? homepageUrl) =>
        new(
            CustomerId.FromPersistence(customerId),
            CompanyName.FromPersistence(companyName),
            CustomerContact.FromPersistence(contactName, contactTitle),
            CustomerAddress.FromPersistence(addressLine, city, region, postalCode, country),
            CustomerCommunication.FromPersistence(phone, fax, homepageUrl));

    public void AssignId(CustomerId id)
    {
        if (Id is not null)
            throw new InvalidOperationException("Customer id is already assigned.");

        Id = id;
    }

    public void Update(
        string companyName,
        string? contactName,
        string? contactTitle,
        string? addressLine,
        string? city,
        string? region,
        string? postalCode,
        string? country,
        string? phone,
        string? fax,
        string? homepageUrl)
    {
        var errors = Validate(companyName, contactName, contactTitle, addressLine, city, region, postalCode, country,
            phone, fax);

        if (errors.Count > 0)
            throw new DomainValidationException(errors);

        CompanyName = CompanyName.Create(companyName);
        Contact = CustomerContact.Create(contactName, contactTitle);
        Address = CustomerAddress.Create(addressLine, city, region, postalCode, country);
        Communication = CustomerCommunication.Create(phone, fax, homepageUrl);
    }

    static List<string> Validate(
        string companyName,
        string? contactName,
        string? contactTitle,
        string? addressLine,
        string? city,
        string? region,
        string? postalCode,
        string? country,
        string? phone,
        string? fax)
    {
        var errors = new List<string>();
        errors.AddRange(CompanyName.Validate(companyName));
        errors.AddRange(CustomerContact.Validate(contactName, contactTitle));
        errors.AddRange(CustomerAddress.Validate(addressLine, city, region, postalCode, country));
        errors.AddRange(CustomerCommunication.Validate(phone, fax));
        return errors;
    }
}

public sealed record CustomerId
{
    CustomerId(string value) => Value = value;

    public string Value { get; }

    public static IReadOnlyList<string> Validate(string? value)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add("CustomerId is required.");
            return errors;
        }

        var normalized = value.Trim().ToUpperInvariant();

        if (normalized.Length != 5)
            errors.Add("CustomerId must be exactly 5 characters.");

        if (normalized.Any(c => !char.IsLetterOrDigit(c)))
            errors.Add("CustomerId must contain only uppercase letters and digits.");

        return errors;
    }

    public static CustomerId Create(string value)
    {
        var errors = Validate(value);
        if (errors.Count > 0)
            throw new DomainValidationException(errors);

        return new CustomerId(value.Trim().ToUpperInvariant());
    }

    public static CustomerId FromPersistence(string value) => new(value);
}

public sealed record CompanyName
{
    CompanyName(string value) => Value = value;

    public string Value { get; }

    public static IReadOnlyList<string> Validate(string? value)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(value))
            errors.Add("CompanyName is required.");
        else if (value.Trim().Length > 40)
            errors.Add("CompanyName cannot exceed 40 characters.");
        return errors;
    }

    public static CompanyName Create(string value) => new(value.Trim());

    public static CompanyName FromPersistence(string value) => new(value);
}

public sealed record CustomerContact
{
    CustomerContact(string? contactName, string? contactTitle)
    {
        ContactName = Normalize(contactName);
        ContactTitle = Normalize(contactTitle);
    }

    public string? ContactName { get; }
    public string? ContactTitle { get; }

    public static IReadOnlyList<string> Validate(string? contactName, string? contactTitle)
    {
        var errors = new List<string>();
        if (Normalize(contactName)?.Length > 30)
            errors.Add("ContactName cannot exceed 30 characters.");
        if (Normalize(contactTitle)?.Length > 30)
            errors.Add("ContactTitle cannot exceed 30 characters.");
        return errors;
    }

    public static CustomerContact Create(string? contactName, string? contactTitle) => new(contactName, contactTitle);

    public static CustomerContact FromPersistence(string? contactName, string? contactTitle) => new(contactName,
        contactTitle);

    static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed record CustomerAddress
{
    CustomerAddress(string? addressLine, string? city, string? region, string? postalCode, string? country)
    {
        AddressLine = Normalize(addressLine);
        City = Normalize(city);
        Region = Normalize(region);
        PostalCode = Normalize(postalCode);
        Country = Normalize(country);
    }

    public string? AddressLine { get; }
    public string? City { get; }
    public string? Region { get; }
    public string? PostalCode { get; }
    public string? Country { get; }

    public static IReadOnlyList<string> Validate(
        string? addressLine,
        string? city,
        string? region,
        string? postalCode,
        string? country)
    {
        var errors = new List<string>();
        if (Normalize(addressLine)?.Length > 60)
            errors.Add("Address cannot exceed 60 characters.");
        if (Normalize(city)?.Length > 15)
            errors.Add("City cannot exceed 15 characters.");
        if (Normalize(region)?.Length > 15)
            errors.Add("Region cannot exceed 15 characters.");
        if (Normalize(postalCode)?.Length > 10)
            errors.Add("PostalCode cannot exceed 10 characters.");
        if (Normalize(country)?.Length > 15)
            errors.Add("Country cannot exceed 15 characters.");
        return errors;
    }

    public static CustomerAddress Create(
        string? addressLine,
        string? city,
        string? region,
        string? postalCode,
        string? country) =>
        new(addressLine, city, region, postalCode, country);

    public static CustomerAddress FromPersistence(
        string? addressLine,
        string? city,
        string? region,
        string? postalCode,
        string? country) =>
        new(addressLine, city, region, postalCode, country);

    static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed record CustomerCommunication
{
    CustomerCommunication(string? phone, string? fax, string? homepageUrl)
    {
        Phone = Normalize(phone);
        Fax = Normalize(fax);
        HomepageUrl = Normalize(homepageUrl);
    }

    public string? Phone { get; }
    public string? Fax { get; }
    public string? HomepageUrl { get; }

    public static IReadOnlyList<string> Validate(string? phone, string? fax)
    {
        var errors = new List<string>();
        if (Normalize(phone)?.Length > 24)
            errors.Add("Phone cannot exceed 24 characters.");
        if (Normalize(fax)?.Length > 24)
            errors.Add("Fax cannot exceed 24 characters.");
        return errors;
    }

    public static CustomerCommunication Create(string? phone, string? fax, string? homepageUrl) =>
        new(phone, fax, homepageUrl);

    public static CustomerCommunication FromPersistence(string? phone, string? fax, string? homepageUrl) =>
        new(phone, fax, homepageUrl);

    static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
