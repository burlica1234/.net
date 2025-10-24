using FluentValidation;
using FluentValidation.AspNetCore;
using Lab4;
using Lab4.Common.Middleware;
using Lab4.Features.Products;
using Lab4.Features.Products.DTOs;

using Lab4.Validators.Attributes;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationContext>(opt => opt.UseInMemoryDatabase("products-db"));
builder.Services.AddMemoryCache();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<IValidator<CreateProductProfileRequest>, CreateProductProfileValidator>();

builder.Services.AddScoped<CreateProductHandler>();

var app = builder.Build();

app.UseMiddleware<CorrelationMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/products", async (CreateProductProfileRequest req, IValidator<CreateProductProfileRequest> v, CreateProductHandler handler, CancellationToken ct) =>
    {
        var res = await v.ValidateAsync(req, ct);
        if (!res.IsValid) return Results.ValidationProblem(res.ToDictionary());

        try
        {
            var dto = await handler.Handle(req, ct);
            return Results.Created($"/products/{dto.Id}", dto);
        }
        catch (InvalidOperationException ex)
        {
            return Results.Conflict(new { error = ex.Message });
        }
    })
    .WithName("CreateProduct")
    .WithOpenApi(op => { op.Summary = "Create product (manual mapping for now)"; return op; });

app.MapGet("/products", async (ApplicationContext db, CancellationToken ct) =>
    {
        var list = await db.Products.OrderByDescending(p => p.CreatedAt).ToListAsync(ct);
        return Results.Ok(list);
    })
    .WithName("ListProducts");

app.Run();