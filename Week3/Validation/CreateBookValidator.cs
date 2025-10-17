
using ConsoleApp2.Contracts;
using FluentValidation;

namespace ConsoleApp2.Validation;

public class CreateBookValidator : AbstractValidator<CreateBookDto>
{
    public CreateBookValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Author).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Year).InclusiveBetween(1450, DateTime.UtcNow.Year + 1);
    }
}