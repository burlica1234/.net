using System.ComponentModel.DataAnnotations;
using Lab4.Features.Products;
using System.Text.RegularExpressions;

namespace Lab4.Validators.Attributes;

public class ValidSKUAttribute : ValidationAttribute
{
    private static readonly Regex Rx = new(@"^[A-Za-z0-9-]{5,20}$", RegexOptions.Compiled);
    protected override ValidationResult? IsValid(object? value, ValidationContext ctx)
    {
        var raw = value?.ToString()?.Replace(" ", "");
        return string.IsNullOrWhiteSpace(raw) || !Rx.IsMatch(raw)
            ? new ValidationResult("SKU must be alphanumeric with hyphens (5-20 chars).")
            : ValidationResult.Success;
    }
}