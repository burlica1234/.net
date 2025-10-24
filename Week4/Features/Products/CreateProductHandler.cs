using Lab4.Common.Logging;
using Lab4.Features.Products.DTOs;
using Microsoft.EntityFrameworkCore;               
using Microsoft.Extensions.Caching.Memory;         
using Microsoft.Extensions.Logging;

namespace Lab4.Features.Products;

public class CreateProductHandler
{
    private readonly ApplicationContext _db;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CreateProductHandler> _logger;

    public CreateProductHandler(
        ApplicationContext db,
        IMemoryCache cache,
        ILogger<CreateProductHandler> logger)
    {
        _db = db;
        _cache = cache;
        _logger = logger;
    }

    public async Task<ProductProfileDto> Handle(CreateProductProfileRequest request, CancellationToken ct = default)
    {
        var opId = Guid.NewGuid().ToString("N")[..8];
        var tStart = DateTime.UtcNow;

        _logger.LogInformation(new EventId(LogEvents.ProductCreationStarted),
            "Create product started: {Name} {Brand} {SKU}", request.Name, request.Brand, request.SKU);

        var t0 = DateTime.UtcNow;
        if (await _db.Products.AnyAsync(p => p.SKU == request.SKU, ct))
        {
            _logger.LogWarning(new EventId(LogEvents.ProductValidationFailed), "SKU already exists: {SKU}", request.SKU);
            throw new InvalidOperationException("SKU already exists.");
        }
        var validationTime = DateTime.UtcNow - t0;

        var entity = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Brand = request.Brand.Trim(),
            SKU = request.SKU.Trim(),
            Category = request.Category,
            Price = request.Price,
            ReleaseDate = request.ReleaseDate,
            ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl,
            StockQuantity = request.StockQuantity,
            IsAvailable = request.StockQuantity > 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        var dbStart = DateTime.UtcNow;
        _db.Products.Add(entity);
        await _db.SaveChangesAsync(ct);
        var dbTime = DateTime.UtcNow - dbStart;

        _cache.Remove("all_products");
        _logger.LogInformation(new EventId(LogEvents.CacheOperationPerformed), "Cache invalidated for key all_products.");

        
        var dto = new ProductProfileDto(
            Id: entity.Id,
            Name: entity.Name,
            Brand: entity.Brand,
            SKU: entity.SKU,
            CategoryDisplayName: entity.Category.ToString(),
            Price: entity.Price,
            FormattedPrice: entity.Price.ToString("C2"),
            ReleaseDate: entity.ReleaseDate,
            CreatedAt: entity.CreatedAt,
            ImageUrl: entity.ImageUrl,
            IsAvailable: entity.IsAvailable,
            StockQuantity: entity.StockQuantity,
            ProductAge: (DateTime.UtcNow.Date - entity.ReleaseDate.Date).TotalDays < 30 ? "New Release" : "Standard",
            BrandInitials: GetBrandInitials(entity.Brand),
            AvailabilityStatus: entity.IsAvailable
                ? (entity.StockQuantity == 0 ? "Unavailable"
                    : entity.StockQuantity == 1 ? "Last Item"
                    : entity.StockQuantity <= 5 ? "Limited Stock"
                    : "In Stock")
                : "Out of Stock"
        );

        _logger.LogProductCreationMetrics(new ProductCreationMetrics(
            opId,
            entity.Name,
            entity.SKU,
            entity.Category,
            validationTime,
            dbTime,
            DateTime.UtcNow - tStart,
            Success: true   // ← S mare
        ));
        _logger.LogInformation(new EventId(LogEvents.ProductCreationCompleted), "Create product completed: {Id}", entity.Id);

        return dto;
    }

    private static string GetBrandInitials(string brand)
    {
        if (string.IsNullOrWhiteSpace(brand)) return "?";
        var parts = brand.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length == 1
            ? parts[0][0].ToString().ToUpperInvariant()
            : $"{char.ToUpperInvariant(parts.First()[0])}{char.ToUpperInvariant(parts.Last()[0])}";
    }
}
