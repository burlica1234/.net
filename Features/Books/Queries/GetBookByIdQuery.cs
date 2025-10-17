using ConsoleApp2.Contracts;
using ConsoleApp2.Data;
using ConsoleApp2.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp2.Features.Books.Queries;

public record GetBookByIdQuery(int Id) : IRequest<BookDto>;

public class GetBookByIdHandler(AppDbContext db) : IRequestHandler<GetBookByIdQuery, BookDto>
{
    public async Task<BookDto> Handle(GetBookByIdQuery r, CancellationToken ct)
    {
        var b = await db.Books.FirstOrDefaultAsync(x => x.Id == r.Id, ct)
                ?? throw new NotFoundException("Book", r.Id);

        return new BookDto(b.Id, b.Title, b.Author, b.Year);
    }
}