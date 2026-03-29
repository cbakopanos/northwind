using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Catalog.Infrastructure;

[Table("categories", Schema = "catalog")]
public sealed class CategoryEntity
{
    [Key]
    [Column("category_id")]
    public int CategoryId { get; set; }

    [Column("category_name")]
    [MaxLength(15)]
    [Required]
    public string CategoryName { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("picture")]
    public byte[]? Picture { get; set; }
}
