
using System.ComponentModel.DataAnnotations;


namespace Lab4.Features.Products;


public class Product
{
    public Guid Id { get; set; }
    [MaxLength(200)] public string Name { get; set; } = default!;
    [MaxLength(100)] public string Brand { get; set; } = default!;
    [MaxLength(20)]  public string SKU { get; set; } = default!;
    public ProductCategory Category { get; set; }
    public decimal Price { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
    public int StockQuantity { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}