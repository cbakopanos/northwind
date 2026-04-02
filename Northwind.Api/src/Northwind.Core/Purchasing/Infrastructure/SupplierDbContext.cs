using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Northwind.Purchasing.Infrastructure;

public sealed class SupplierDbContext(DbContextOptions<SupplierDbContext> options) : DbContext(options)
{
    public DbSet<SupplierEntity> Suppliers => Set<SupplierEntity>();
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

    [Column("address")] [MaxLength(60)] public string? Address { get; set; }

    [Column("city")] [MaxLength(15)] public string? City { get; set; }

    [Column("region")] [MaxLength(15)] public string? Region { get; set; }

    [Column("postal_code")]
    [MaxLength(10)]
    public string? PostalCode { get; set; }

    [Column("country")] [MaxLength(15)] public string? Country { get; set; }

    [Column("phone")] [MaxLength(24)] public string? Phone { get; set; }

    [Column("fax")] [MaxLength(24)] public string? Fax { get; set; }

    [Column("homepage_url")] public string? HomepageUrl { get; set; }
}