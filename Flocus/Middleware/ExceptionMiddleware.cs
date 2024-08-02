using AutoMapper;
using Flocus.Domain.Models.Errors;
using Flocus.Identity.Exceptions;
using Flocus.Models.Errors;
using Flocus.Repository.Exceptions;
using System.Security.Authentication;

namespace Flocus.Middleware;

public sealed class ExceptionMiddleware : IMiddleware
{
    private const string UnhandledExceptionTitle = "An unhandled exception occurred.";

    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IMapper _mapper;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, IMapper mapper)
    {
        _logger = logger;
        _mapper = mapper;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (DuplicateRecordException ex)
        {
            await WriteSingleErrorResponseAsync(context, StatusCodes.Status409Conflict, ex.Message);
        }
        catch (RecordNotFoundException ex)
        {
            await WriteSingleErrorResponseAsync(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (InputValidationException ex)
        {
            await WriteMultiErrorResponseAsync(context, MapErrorsToErrorDtos(ex.Errors));
        }
        catch (AuthenticationException ex)
        {
            await WriteSingleErrorResponseAsync(context, StatusCodes.Status401Unauthorized, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteSingleErrorResponseAsync(context, StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (Exception)
        {
            await WriteSingleErrorResponseAsync(context, StatusCodes.Status500InternalServerError, UnhandledExceptionTitle);
        }
    }

    #region Private Methods
    private async Task WriteSingleErrorResponseAsync(HttpContext context, int status, string message)
    {
        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(
            new ErrorsDto(
                new List<ErrorDto>
                {
                    new ErrorDto(status, message)
                }));
    }

    private async Task WriteMultiErrorResponseAsync(HttpContext context, List<ErrorDto> errors)
    {
        context.Response.StatusCode = GetAppropriateStatusCode(errors);
        await context.Response.WriteAsJsonAsync(
            new ErrorsDto(errors));
    }

    private int GetAppropriateStatusCode(List<ErrorDto> errors)
    {
        var firstStatus = errors[0].Status;
        if (errors.All(x => x.Status == firstStatus))
        {
            return firstStatus;
        }

        return StatusCodes.Status207MultiStatus;
    }

    private List<ErrorDto> MapErrorsToErrorDtos(List<Error> errors)
    {
        var errorDtos = _mapper.Map<List<ErrorDto>>(errors);
        return errorDtos;
    }
    #endregion
}
