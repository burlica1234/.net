using FluentValidation;
using Lab4.Features.Products.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Lab4.Validators.Attributes;

public class CreateProductProfileValidator : AbstractValidator<CreateProductProfileRequest>
{
    private readonly ApplicationContext _db;
    public CreateProductProfileValidator(ApplicationContext db)
    {
        _db = db;

        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Brand).NotEmpty().MaximumLength(100);

        RuleFor(x => x.SKU)
            .NotEmpty().MaximumLength(20)
            .Matches(@"^[A-Za-z0-9-]{5,20}$").WithMessage("SKU must be alphanumeric with hyphens (5-20 chars).")
            .MustAsync(async (sku, ct) => !await _db.Products.AnyAsync(p => p.SKU == sku, ct))
            .WithMessage("SKU already exists.");

        RuleFor(x => x.Category).IsInEnum();
        RuleFor(x => x.Price).GreaterThan(0).LessThanOrEqualTo(100_000);
        RuleFor(x => x.ReleaseDate).LessThanOrEqualTo(DateTime.UtcNow.Date);
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0).LessThanOrEqualTo(100_000);

        When(x => !string.IsNullOrWhiteSpace(x.ImageUrl), () =>
        {
            RuleFor(x => x.ImageUrl!)
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var u)
                             && (u.Scheme is "http" or "https")
                             && new[] { ".jpg",".jpeg",".png",".gif",".webp" }
                                 .Any(ext => u.AbsolutePath.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                .WithMessage("ImageUrl must be http(s) and end with an image extension.");
        });
    }
}