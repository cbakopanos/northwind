namespace Northwind.Catalog.Domain;

public sealed class Product
{
    Product(
        ProductId? id,
        ProductName productName,
        int? supplierId,
        int? categoryId,
        QuantityPerUnit quantityPerUnit,
        UnitPrice unitPrice,
        Inventory inventory,
        ReorderLevel reorderLevel,
        bool isDiscontinued,
        string? categoryName,
        string? supplierDisplayName)
    {
        Id = id;
        ProductName = productName;
        SupplierId = supplierId;
        CategoryId = categoryId;
        QuantityPerUnit = quantityPerUnit;
        UnitPrice = unitPrice;
        Inventory = inventory;
        ReorderLevel = reorderLevel;
        IsDiscontinued = isDiscontinued;
        CategoryName = categoryName;
        SupplierDisplayName = supplierDisplayName;
    }

    public ProductId? Id { get; private set; }
    public ProductName ProductName { get; private set; }
    public int? SupplierId { get; private set; }
    public int? CategoryId { get; private set; }
    public QuantityPerUnit QuantityPerUnit { get; private set; }
    public UnitPrice UnitPrice { get; private set; }
    public Inventory Inventory { get; private set; }
    public ReorderLevel ReorderLevel { get; private set; }
    public bool IsDiscontinued { get; private set; }

    // Read-only projection data (not business invariants)
    public string? CategoryName { get; }
    public string? SupplierDisplayName { get; }

    public static Product Create(
        string productName,
        int? supplierId,
        int? categoryId,
        string? quantityPerUnit,
        decimal unitPrice,
        short unitsInStock,
        short unitsOnOrder,
        short reorderLevel)
    {
        var errors = Validate(productName, quantityPerUnit, unitPrice, unitsInStock, unitsOnOrder, reorderLevel);
        if (errors.Count > 0)
            throw new DomainValidationException(errors);

        return new Product(
            null,
            ProductName.Create(productName),
            supplierId,
            categoryId,
            QuantityPerUnit.Create(quantityPerUnit),
            UnitPrice.Create(unitPrice),
            Inventory.Create(unitsInStock, unitsOnOrder),
            ReorderLevel.Create(reorderLevel),
            false,
            null,
            null);
    }

    public static Product Rehydrate(
        int productId,
        string productName,
        int? supplierId,
        int? categoryId,
        string? quantityPerUnit,
        decimal unitPrice,
        short unitsInStock,
        short unitsOnOrder,
        short reorderLevel,
        bool isDiscontinued,
        string? categoryName,
        string? supplierDisplayName) =>
        new(
            ProductId.FromPersistence(productId),
            ProductName.FromPersistence(productName),
            supplierId,
            categoryId,
            QuantityPerUnit.FromPersistence(quantityPerUnit),
            UnitPrice.FromPersistence(unitPrice),
            Inventory.FromPersistence(unitsInStock, unitsOnOrder),
            ReorderLevel.FromPersistence(reorderLevel),
            isDiscontinued,
            categoryName,
            supplierDisplayName);

    public void AssignId(ProductId id)
    {
        if (Id is not null)
            throw new InvalidOperationException("Product id is already assigned.");

        Id = id;
    }

    public void UpdateDetails(
        string productName,
        int? supplierId,
        int? categoryId,
        string? quantityPerUnit,
        decimal unitPrice,
        short unitsInStock,
        short unitsOnOrder,
        short reorderLevel)
    {
        var errors = Validate(productName, quantityPerUnit, unitPrice, unitsInStock, unitsOnOrder, reorderLevel);
        if (errors.Count > 0)
            throw new DomainValidationException(errors);

        ProductName = ProductName.Create(productName);
        SupplierId = supplierId;
        CategoryId = categoryId;
        QuantityPerUnit = QuantityPerUnit.Create(quantityPerUnit);
        UnitPrice = UnitPrice.Create(unitPrice);
        Inventory = Inventory.Create(unitsInStock, unitsOnOrder);
        ReorderLevel = ReorderLevel.Create(reorderLevel);
    }

    public void Discontinue() => IsDiscontinued = true;

    public void Reinstate() => IsDiscontinued = false;

    static List<string> Validate(
        string productName,
        string? quantityPerUnit,
        decimal unitPrice,
        short unitsInStock,
        short unitsOnOrder,
        short reorderLevel)
    {
        var errors = new List<string>();
        errors.AddRange(ProductName.Validate(productName));
        errors.AddRange(QuantityPerUnit.Validate(quantityPerUnit));
        errors.AddRange(UnitPrice.Validate(unitPrice));
        errors.AddRange(Inventory.Validate(unitsInStock, unitsOnOrder));
        errors.AddRange(ReorderLevel.Validate(reorderLevel));
        return errors;
    }
}

public sealed record ProductId
{
    ProductId(int value) => Value = value;

    public int Value { get; }

    public static IReadOnlyList<string> Validate(int value)
    {
        var errors = new List<string>();
        if (value <= 0)
            errors.Add("ProductId must be greater than zero.");
        return errors;
    }

    public static ProductId Create(int value)
    {
        var errors = Validate(value);
        if (errors.Count > 0)
            throw new DomainValidationException(errors);

        return new ProductId(value);
    }

    public static ProductId FromPersistence(int value) => new(value);
}

public sealed record ProductName
{
    ProductName(string value) => Value = value;

    public string Value { get; }

    public static IReadOnlyList<string> Validate(string? value)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(value))
            errors.Add("ProductName is required.");
        else if (value.Trim().Length > 40)
            errors.Add("ProductName cannot exceed 40 characters.");
        return errors;
    }

    public static ProductName Create(string value) => new(value.Trim());

    public static ProductName FromPersistence(string value) => new(value);
}

public sealed record QuantityPerUnit
{
    QuantityPerUnit(string? value) => Value = Normalize(value);

    public string? Value { get; }

    public static IReadOnlyList<string> Validate(string? value)
    {
        var errors = new List<string>();
        if (Normalize(value)?.Length > 20)
            errors.Add("QuantityPerUnit cannot exceed 20 characters.");
        return errors;
    }

    public static QuantityPerUnit Create(string? value) => new(value);

    public static QuantityPerUnit FromPersistence(string? value) => new(value);

    static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed record UnitPrice
{
    UnitPrice(decimal value) => Value = value;

    public decimal Value { get; }

    public static IReadOnlyList<string> Validate(decimal value)
    {
        var errors = new List<string>();
        if (value < 0)
            errors.Add("UnitPrice must be non-negative.");
        return errors;
    }

    public static UnitPrice Create(decimal value)
    {
        var errors = Validate(value);
        if (errors.Count > 0)
            throw new DomainValidationException(errors);

        return new UnitPrice(value);
    }

    public static UnitPrice FromPersistence(decimal value) => new(value);
}

public sealed record Inventory
{
    Inventory(short unitsInStock, short unitsOnOrder)
    {
        UnitsInStock = unitsInStock;
        UnitsOnOrder = unitsOnOrder;
    }

    public short UnitsInStock { get; }
    public short UnitsOnOrder { get; }

    public static IReadOnlyList<string> Validate(short unitsInStock, short unitsOnOrder)
    {
        var errors = new List<string>();
        if (unitsInStock < 0)
            errors.Add("UnitsInStock must be non-negative.");
        if (unitsOnOrder < 0)
            errors.Add("UnitsOnOrder must be non-negative.");
        return errors;
    }

    public static Inventory Create(short unitsInStock, short unitsOnOrder)
    {
        var errors = Validate(unitsInStock, unitsOnOrder);
        if (errors.Count > 0)
            throw new DomainValidationException(errors);

        return new Inventory(unitsInStock, unitsOnOrder);
    }

    public static Inventory FromPersistence(short unitsInStock, short unitsOnOrder) => new(unitsInStock,
        unitsOnOrder);
}

public sealed record ReorderLevel
{
    ReorderLevel(short value) => Value = value;

    public short Value { get; }

    public static IReadOnlyList<string> Validate(short value)
    {
        var errors = new List<string>();
        if (value < 0)
            errors.Add("ReorderLevel must be non-negative.");
        return errors;
    }

    public static ReorderLevel Create(short value)
    {
        var errors = Validate(value);
        if (errors.Count > 0)
            throw new DomainValidationException(errors);

        return new ReorderLevel(value);
    }

    public static ReorderLevel FromPersistence(short value) => new(value);
}
