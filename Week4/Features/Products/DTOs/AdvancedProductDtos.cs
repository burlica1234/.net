using System.ComponentModel.DataAnnotations;

namespace Lab4.Features.Products.DTOs;


public record CreateProductProfileRequest(
    [Required, MaxLength(200)] string Name,
    [Required, MaxLength(100)] string Brand,
    [Required, MaxLength(20)]  string SKU,
    ProductCategory Category,
    [Range(0.01, 100000)] decimal Price,
    DateTime ReleaseDate,
    string? ImageUrl,
    [Range(0, 100000)] int StockQuantity
);

public record ProductProfileDto(
    Guid Id,
    string Name,
    string Brand,
    string SKU,
    string CategoryDisplayName,
    decimal Price,
    string FormattedPrice,
    DateTime ReleaseDate,
    DateTime CreatedAt,
    string? ImageUrl,
    bool IsAvailable,
    int StockQuantity,
    string ProductAge,
    string BrandInitials,
    string AvailabilityStatus
);