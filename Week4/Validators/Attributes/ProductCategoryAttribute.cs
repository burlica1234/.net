
using System.ComponentModel.DataAnnotations;
using Lab4.Features.Products;

namespace Lab4.Validators.Attributes;


public class ProductCategoryAttribute : ValidationAttribute
{
    private readonly ProductCategory[] _allowed;
    public ProductCategoryAttribute(params ProductCategory[] allowed) => _allowed = allowed;

    public override bool IsValid(object? value)
        => value is ProductCategory c && _allowed.Contains(c);

    public override string FormatErrorMessage(string name)
        => $"{name} must be one of: {string.Join(", ", _allowed)}";
}