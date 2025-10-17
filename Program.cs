using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediatR;
using FluentValidation;

using ConsoleApp2.Data;
using ConsoleApp2.Domain;
using ConsoleApp2.Contracts;
using ConsoleApp2.Validation;
using ConsoleApp2.Exceptions;
using ConsoleApp2.Sorting;
using ConsoleApp2.Features.Books.Commands;
using ConsoleApp2.Features.Books.Queries;

var builder = WebApplication.CreateBuilder(args);

// EF Core – bază de date InMemory pentru demo
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("books-db"));

// MediatR v12 – înregistrare CQRS handlers
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GetBooksQuery).Assembly));

// FluentValidation – înregistrează validatorii automat
builder.Services.AddValidatorsFromAssemblyContaining<CreateBookValidator>();

var app = builder.Build();

// Middleware global pentru gestionarea excepțiilor
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Seed de date demo (3 cărți)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.Books.Any())
    {
        db.Books.AddRange(
            new Book { Title = "Clean Code", Author = "Robert C. Martin", Year = 2008 },
            new Book { Title = "Domain-Driven Design", Author = "Eric Evans", Year = 2003 },
            new Book { Title = "The Pragmatic Programmer", Author = "Andrew Hunt", Year = 1999 }
        );
        db.SaveChanges();
    }
}

// ✅ Endpoints API (CQRS prin MediatR)

// GET /api/books  — listă, filtrare, paginare, sortare
app.MapGet("/api/books", async (
    [FromServices] IMediator mediator,
    string? author = null,
    int page = 1,
    int pageSize = 10,
    SortBy sortBy = SortBy.Title,
    SortDir sortDir = SortDir.Asc) =>
{
    page = page <= 0 ? 1 : page;
    pageSize = pageSize is <= 0 or > 200 ? 10 : pageSize;

    var result = await mediator.Send(new GetBooksQuery(author, page, pageSize, sortBy, sortDir));
    return Results.Ok(result);
});


app.MapGet("/api/books/{id:int}", async (int id, [FromServices] IMediator mediator) =>
{
    var dto = await mediator.Send(new GetBookByIdQuery(id));
    return Results.Ok(dto);
});


app.MapPost("/api/books", async ([FromServices] IMediator mediator, CreateBookDto dto) =>
{
    var created = await mediator.Send(new CreateBookCommand(dto));
    return Results.Created($"/api/books/{created.Id}", created);
});


app.MapPut("/api/books/{id:int}", async ([FromServices] IMediator mediator, int id, UpdateBookDto dto) =>
{
    var updated = await mediator.Send(new UpdateBookCommand(id, dto));
    return Results.Ok(updated);
});


app.MapDelete("/api/books/{id:int}", async ([FromServices] IMediator mediator, int id) =>
{
    await mediator.Send(new DeleteBookCommand(id));
    return Results.NoContent();
});


app.MapGet("/", () => "OK");


app.Run();
