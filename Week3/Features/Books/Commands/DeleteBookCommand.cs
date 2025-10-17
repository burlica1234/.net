using ConsoleApp2.Data;
using ConsoleApp2.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp2.Features.Books.Commands;

public record DeleteBookCommand(int Id) : IRequest<Unit>;

public class DeleteBookHandler(AppDbContext db) : IRequestHandler<DeleteBookCommand, Unit>
{
    public async Task<Unit> Handle(DeleteBookCommand r, CancellationToken ct)
    {
        var b = await db.Books.FirstOrDefaultAsync(x => x.Id == r.Id, ct)
                ?? throw new NotFoundException("Book", r.Id);

        db.Books.Remove(b);
        await db.SaveChangesAsync(ct);

        return Unit.Value;
    }
}