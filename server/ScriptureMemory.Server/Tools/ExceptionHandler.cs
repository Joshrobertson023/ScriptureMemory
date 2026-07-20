using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ScriptureMemory.Server.CustomExceptions;

namespace ScriptureMemory.Server.Tools;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            BookNotFoundException bookNotFoundException => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Invalid book",
                Detail = bookNotFoundException.Message,
                Instance = httpContext.Request.Path,
                Extensions = new Dictionary<string, object?>()
                {
                    ["book"] = bookNotFoundException.Book
                }
            },
            InvalidChapterException invalidChapterException => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Invalid chapter number",
                Detail = invalidChapterException.Message,
                Instance = httpContext.Request.Path,
                Extensions = new Dictionary<string, object?>()
                {
                    ["book"] = invalidChapterException.Book,
                    ["chapter"] = invalidChapterException.Chapter
                }
            },
            BibleUnavailableException bibleUnavailableException => new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Bible unavailable",
                Detail = bibleUnavailableException.Message,
                Instance = httpContext.Request.Path,
                Extensions = new Dictionary<string, object?>()
                {
                    ["bible"] = bibleUnavailableException.Bible
                }
            },
            
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal server error",
                Detail = "An unexpected error occurred.",
                Instance = httpContext.Request.Path
            }
        };
        
        if (problemDetails.Status >= 500)
            _logger.LogError(
                exception,
                "Unhandled exception: {ExceptionMessage} while processing {Method} {Path}. TraceId: {TraceId}, User: {User}",
                exception.Message,
                httpContext.Request.Method,
                httpContext.Request.Path,
                httpContext.TraceIdentifier,
                httpContext.User.Identity?.Name ?? "Anonymous");
        else
            _logger.LogWarning(
                exception,
                "Request failed ({Title}) with client error {StatusCode}: {Message} {Path}. TraceId: {TraceId}, User: {User}",
                problemDetails.Title,
                problemDetails.Status,
                exception.Message,
                httpContext.Request.Path,
                httpContext.TraceIdentifier,
                httpContext.User.Identity?.Name ?? "Anonymous");

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}