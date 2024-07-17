using Flocus.Identity.Exceptions;
using Flocus.Models.Errors;
using Flocus.Repository.Exceptions;
using System.Security.Authentication;

namespace Flocus.Middleware;

public sealed class ExceptionMiddleware : IMiddleware
{
    private const string UnhandledExceptionTitle = "An unhandled exception occurred.";


    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (DuplicateRecordException ex)
        {
            await WriteResponseAsync(context, StatusCodes.Status409Conflict, ex.Message);
        }
        catch (RecordNotFoundException ex)
        {
            await WriteResponseAsync(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (InputValidationException ex)
        {
            await WriteResponseAsync(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (AuthenticationException ex)
        {
            await WriteResponseAsync(context, StatusCodes.Status401Unauthorized, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteResponseAsync(context, StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (Exception ex)
        {
            await WriteResponseAsync(context, StatusCodes.Status500InternalServerError, UnhandledExceptionTitle);
        }
    }

    private static async Task WriteResponseAsync(HttpContext context, int status, string message)
    {
        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(
            new ErrorsDto(
                new List<ErrorDto>
                {
                    new ErrorDto(status, message)
                }));
    }
}
