using System.ComponentModel.DataAnnotations;

namespace Lab4.Validators.Attributes;

public class PriceRangeAttribute : ValidationAttribute
{
    private readonly decimal _min, _max;
    public PriceRangeAttribute(double min, double max) { _min = (decimal)min; _max = (decimal)max; }

    public override bool IsValid(object? value)
        => value is decimal d && d >= _min && d <= _max;

    public override string FormatErrorMessage(string name)
        => $"{name} must be between {_min:C2} and {_max:C2}";
}