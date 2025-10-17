using FluentValidation.Results;

namespace ConsoleApp2.Exceptions;

public class ApiValidationException : Exception
{
    public Dictionary<string, string[]> Errors { get; }
    public ApiValidationException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation errors occurred.")
    {
        Errors = failures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).ToArray());
    }
}