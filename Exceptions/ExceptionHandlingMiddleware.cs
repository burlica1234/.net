using System.Net;

namespace ConsoleApp2.Exceptions;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (NotFoundException nf)
        {
            _logger.LogWarning(nf, "Not found");
            ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await ctx.Response.WriteAsJsonAsync(new { error = nf.Message });
        }
        catch (ApiValidationException ve)
        {
            _logger.LogInformation("Validation error: {Errors}", ve.Errors);
            ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await ctx.Response.WriteAsJsonAsync(new
            {
                error = "ValidationFailed",
                details = ve.Errors
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await ctx.Response.WriteAsJsonAsync(new { error = "ServerError" });
        }
    }
}