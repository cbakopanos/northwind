using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Catalog.Infrastructure;

public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();
    public DbSet<ProductEntity> Products => Set<ProductEntity>();
    public DbSet<SupplierEntity> Suppliers => Set<SupplierEntity>();
}

[Table("categories", Schema = "catalog")]
public sealed class CategoryEntity
{
    [Key] [Column("category_id")] public int CategoryId { get; set; }

    [Column("category_name")]
    [MaxLength(15)]
    [Required]
    public string CategoryName { get; set; } = string.Empty;

    [Column("description")] public string? Description { get; set; }

    [Column("created_at")] public DateTimeOffset CreatedAt { get; set; }

    [Column("picture")] public byte[]? Picture { get; set; }
}

[Table("products", Schema = "catalog")]
public sealed class ProductEntity
{
    [Key] [Column("product_id")] public int ProductId { get; set; }

    [Column("product_name")]
    [MaxLength(40)]
    [Required]
    public string ProductName { get; set; } = string.Empty;

    [Column("supplier_id")] public int? SupplierId { get; set; }

    [Column("category_id")] public int? CategoryId { get; set; }

    [Column("quantity_per_unit")]
    [MaxLength(20)]
    public string? QuantityPerUnit { get; set; }

    [Column("unit_price")] public decimal UnitPrice { get; set; }

    [Column("units_in_stock")] public short UnitsInStock { get; set; }

    [Column("units_on_order")] public short UnitsOnOrder { get; set; }

    [Column("reorder_level")] public short ReorderLevel { get; set; }

    [Column("created_at")] public DateTimeOffset CreatedAt { get; set; }

    [Column("is_discontinued")] public bool IsDiscontinued { get; set; }

    [ForeignKey(nameof(CategoryId))] public CategoryEntity? Category { get; set; }
    [ForeignKey(nameof(SupplierId))] public SupplierEntity? Supplier { get; set; }
}

[Table("suppliers", Schema = "purchasing")]
public sealed class SupplierEntity
{
    [Key] [Column("supplier_id")] public int SupplierId { get; init; }

    [Column("company_name")]
    [MaxLength(40)]
    [Required]
    public string CompanyName { get; set; } = string.Empty;

    [Column("contact_name")]
    [MaxLength(30)]
    public string? ContactName { get; set; }

    [Column("contact_title")]
    [MaxLength(30)]
    public string? ContactTitle { get; set; }

    [Column("created_at")] public DateTimeOffset CreatedAt { get; set; }
    
}