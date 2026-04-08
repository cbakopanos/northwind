namespace Northwind.Purchasing.Domain;

public sealed class Supplier
{
    Supplier(
        SupplierId? id,
        CompanyName companyName,
        SupplierContact contact,
        SupplierAddress address,
        SupplierCommunication communication)
    {
        Id = id;
        CompanyName = companyName;
        Contact = contact;
        Address = address;
        Communication = communication;
    }

    public SupplierId? Id { get; private set; }
    public CompanyName CompanyName { get; private set; }
    public SupplierContact Contact { get; private set; }
    public SupplierAddress Address { get; private set; }
    public SupplierCommunication Communication { get; private set; }

    public static Supplier Create(
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
        var errors = Validate(companyName, contactName, contactTitle, addressLine, city, region, postalCode,
            country, phone, fax);

        if (errors.Count > 0)
            throw new DomainValidationException(errors);

        return new Supplier(
            null,
            CompanyName.Create(companyName),
            SupplierContact.Create(contactName, contactTitle),
            SupplierAddress.Create(addressLine, city, region, postalCode, country),
            SupplierCommunication.Create(phone, fax, homepageUrl));
    }

    public static Supplier Rehydrate(
        int supplierId,
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
            SupplierId.FromPersistence(supplierId),
            CompanyName.FromPersistence(companyName),
            SupplierContact.FromPersistence(contactName, contactTitle),
            SupplierAddress.FromPersistence(addressLine, city, region, postalCode, country),
            SupplierCommunication.FromPersistence(phone, fax, homepageUrl));

    public void AssignId(SupplierId id)
    {
        if (Id is not null)
            throw new InvalidOperationException("Supplier id is already assigned.");

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
        var errors = Validate(companyName, contactName, contactTitle, addressLine, city, region, postalCode,
            country, phone, fax);

        if (errors.Count > 0)
            throw new DomainValidationException(errors);

        CompanyName = CompanyName.Create(companyName);
        Contact = SupplierContact.Create(contactName, contactTitle);
        Address = SupplierAddress.Create(addressLine, city, region, postalCode, country);
        Communication = SupplierCommunication.Create(phone, fax, homepageUrl);
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
        errors.AddRange(SupplierContact.Validate(contactName, contactTitle));
        errors.AddRange(SupplierAddress.Validate(addressLine, city, region, postalCode, country));
        errors.AddRange(SupplierCommunication.Validate(phone, fax));
        return errors;
    }
}

public sealed record SupplierId
{
    SupplierId(int value) => Value = value;

    public int Value { get; }

    public static IReadOnlyList<string> Validate(int value)
    {
        var errors = new List<string>();
        if (value <= 0)
            errors.Add("SupplierId must be greater than zero.");
        return errors;
    }

    public static SupplierId Create(int value)
    {
        var errors = Validate(value);
        if (errors.Count > 0)
            throw new DomainValidationException(errors);

        return new SupplierId(value);
    }

    public static SupplierId FromPersistence(int value) => new(value);
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

public sealed record SupplierContact
{
    SupplierContact(string? contactName, string? contactTitle)
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

    public static SupplierContact Create(string? contactName, string? contactTitle) => new(contactName, contactTitle);

    public static SupplierContact FromPersistence(string? contactName, string? contactTitle) =>
        new(contactName, contactTitle);

    static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed record SupplierAddress
{
    SupplierAddress(string? addressLine, string? city, string? region, string? postalCode, string? country)
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

    public static SupplierAddress Create(
        string? addressLine,
        string? city,
        string? region,
        string? postalCode,
        string? country) =>
        new(addressLine, city, region, postalCode, country);

    public static SupplierAddress FromPersistence(
        string? addressLine,
        string? city,
        string? region,
        string? postalCode,
        string? country) =>
        new(addressLine, city, region, postalCode, country);

    static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed record SupplierCommunication
{
    SupplierCommunication(string? phone, string? fax, string? homepageUrl)
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

    public static SupplierCommunication Create(string? phone, string? fax, string? homepageUrl) =>
        new(phone, fax, homepageUrl);

    public static SupplierCommunication FromPersistence(string? phone, string? fax, string? homepageUrl) =>
        new(phone, fax, homepageUrl);

    static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
