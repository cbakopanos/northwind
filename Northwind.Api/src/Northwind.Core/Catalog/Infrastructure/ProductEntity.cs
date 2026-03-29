using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Catalog.Infrastructure;

[Table("products", Schema = "catalog")]
public sealed class ProductEntity
{
    [Key]
    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("product_name")]
    [MaxLength(40)]
    [Required]
    public string ProductName { get; set; } = string.Empty;

    [Column("supplier_id")]
    public int? SupplierId { get; set; }

    [Column("category_id")]
    public int? CategoryId { get; set; }

    [Column("quantity_per_unit")]
    [MaxLength(20)]
    public string? QuantityPerUnit { get; set; }

    [Column("unit_price")]
    public decimal UnitPrice { get; set; }

    [Column("units_in_stock")]
    public short UnitsInStock { get; set; }

    [Column("units_on_order")]
    public short UnitsOnOrder { get; set; }

    [Column("reorder_level")]
    public short ReorderLevel { get; set; }

    [Column("is_discontinued")]
    public bool IsDiscontinued { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public CategoryEntity? Category { get; set; }
}
