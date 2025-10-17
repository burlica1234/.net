using ConsoleApp2.Contracts;
using ConsoleApp2.Data;
using ConsoleApp2.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp2.Features.Books.Commands;

public record UpdateBookCommand(int Id, UpdateBookDto Dto) : IRequest<BookDto>;

public class UpdateBookHandler(AppDbContext db, IValidator<UpdateBookDto> validator)
    : IRequestHandler<UpdateBookCommand, BookDto>
{
    public async Task<BookDto> Handle(UpdateBookCommand r, CancellationToken ct)
    {
        var res = await validator.ValidateAsync(r.Dto, ct);
        if (!res.IsValid)
            throw new ApiValidationException(res.Errors);

        var b = await db.Books.FirstOrDefaultAsync(x => x.Id == r.Id, ct)
                ?? throw new NotFoundException("Book", r.Id);

        b.Title = r.Dto.Title;
        b.Author = r.Dto.Author;
        b.Year = r.Dto.Year;

        await db.SaveChangesAsync(ct);

        return new BookDto(b.Id, b.Title, b.Author, b.Year);
    }
}