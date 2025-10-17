using ConsoleApp2.Contracts;
using ConsoleApp2.Data;
using ConsoleApp2.Sorting;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp2.Features.Books.Queries;

public record GetBooksQuery(
    string? Author,
    int Page = 1,
    int PageSize = 10,
    SortBy SortBy = SortBy.Title,
    SortDir SortDir = SortDir.Asc
) : IRequest<PagedResult<BookDto>>;

public class GetBooksHandler(AppDbContext db) : IRequestHandler<GetBooksQuery, PagedResult<BookDto>>
{
    public async Task<PagedResult<BookDto>> Handle(GetBooksQuery r, CancellationToken ct)
    {
        var q = db.Books.AsQueryable();

        if (!string.IsNullOrWhiteSpace(r.Author))
        {
            var author = r.Author.Trim();
            q = q.Where(b => EF.Functions.Like(b.Author, $"%{author}%"));
        }

        q = BookSort.Apply(q, r.SortBy, r.SortDir);

        var total = await q.CountAsync(ct);
        var items = await q
            .Skip((r.Page - 1) * r.PageSize)
            .Take(r.PageSize)
            .Select(b => new BookDto(b.Id, b.Title, b.Author, b.Year))
            .ToListAsync(ct);

        return new(items, r.Page, r.PageSize, total);
    }
}