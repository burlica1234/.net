using ConsoleApp2.Contracts;
using ConsoleApp2.Data;
using ConsoleApp2.Domain;
using ConsoleApp2.Exceptions;
using FluentValidation;
using MediatR;

namespace ConsoleApp2.Features.Books.Commands;

public record CreateBookCommand(CreateBookDto Dto) : IRequest<BookDto>;

public class CreateBookHandler(AppDbContext db, IValidator<CreateBookDto> validator)
    : IRequestHandler<CreateBookCommand, BookDto>
{
    public async Task<BookDto> Handle(CreateBookCommand r, CancellationToken ct)
    {
        var res = await validator.ValidateAsync(r.Dto, ct);
        if (!res.IsValid)
            throw new ApiValidationException(res.Errors);

        var book = new Book
        {
            Title = r.Dto.Title,
            Author = r.Dto.Author,
            Year = r.Dto.Year
        };

        db.Books.Add(book);
        await db.SaveChangesAsync(ct);

        return new BookDto(book.Id, book.Title, book.Author, book.Year);
    }
}