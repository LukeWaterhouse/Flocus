using AutoMapper;
using Flocus.Domain.Models.Errors;
using Flocus.Identity.Exceptions;
using Flocus.Mapping;
using Flocus.Middleware;
using Flocus.Models.Errors;
using Flocus.Repository.Exceptions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Security.Authentication;
using System.Text.Json;
using Xunit;

namespace Flocus.Tests.Middleware;

public class ExceptionMiddlewareTests
{
    private readonly ILogger<ExceptionMiddleware> _loggerMock;
    private readonly ExceptionMiddleware _exceptionMiddleware;
    private readonly IMapper _mapper;

    private readonly string ContentType = "application/json; charset=utf-8";

    public ExceptionMiddlewareTests()
    {
        _loggerMock = Substitute.For<ILogger<ExceptionMiddleware>>();
        var mappingConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(ErrorDtoMappingProfile));
        });
        _mapper = mappingConfig.CreateMapper();
        _exceptionMiddleware = new ExceptionMiddleware(_loggerMock, _mapper);
    }

    [Fact]
    public async Task InvokeAsync_DelegateThrowsDuplicateRecordException_Generates409()
    {
        // Arrange
        var errorMessage = "duplicate record found.";
        var duplicateRecordException = new DuplicateRecordException(errorMessage);
        var (next, context) = GenerateDelegateAndContextFromException(duplicateRecordException);

        var expectedResponseBody = JsonSerializer.Serialize(new ErrorsDto(
            new List<ErrorDto>
            {
            new ErrorDto(StatusCodes.Status409Conflict, errorMessage)
            }));

        // Act
        await _exceptionMiddleware.InvokeAsync(context, next);

        // Assert
        using (new AssertionScope())
        {
            var responseBody = await DeserializeResponseBody(context);
            context.Response.ContentType.Should().Be(ContentType);
            context.Response.StatusCode.Should().Be(StatusCodes.Status409Conflict);
            responseBody.Should().BeEquivalentTo(expectedResponseBody.ToLower());
        }
    }

    [Fact]
    public async Task InvokeAsync_DelegateThrowsRecordNotFoundException_Generates404()
    {
        // Arrange
        var errorMessage = "record not found.";
        var recordNotFoundException = new RecordNotFoundException(errorMessage);
        var (next, context) = GenerateDelegateAndContextFromException(recordNotFoundException);

        var expectedResponseBody = JsonSerializer.Serialize(new ErrorsDto(
            new List<ErrorDto>
            {
            new ErrorDto(StatusCodes.Status404NotFound, errorMessage)
            }));

        // Act
        await _exceptionMiddleware.InvokeAsync(context, next);

        // Assert
        using (new AssertionScope())
        {
            var responseBody = await DeserializeResponseBody(context);
            context.Response.ContentType.Should().Be(ContentType);
            context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            responseBody.Should().BeEquivalentTo(expectedResponseBody.ToLower());
        }
    }

    [Fact]
    public async Task InvokeAsync_DelegateThrowsAuthenticationException_Generates401()
    {
        // Arrange
        var errorMessage = "password username combo incorrect.";
        var authenticationException = new AuthenticationException(errorMessage);
        var (next, context) = GenerateDelegateAndContextFromException(authenticationException);

        var expectedResponseBody = JsonSerializer.Serialize(new ErrorsDto(
            new List<ErrorDto>
            {
            new ErrorDto(StatusCodes.Status401Unauthorized, errorMessage)
            }));

        // Act
        await _exceptionMiddleware.InvokeAsync(context, next);

        // Assert
        using (new AssertionScope())
        {
            var responseBody = await DeserializeResponseBody(context);
            context.Response.ContentType.Should().Be(ContentType);
            context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            responseBody.Should().BeEquivalentTo(expectedResponseBody.ToLower());
        }
    }

    [Fact]
    public async Task InvokeAsync_DelegateThrowsUnAuthorizedAccessException_Generates403()
    {
        // Arrange
        var errorMessage = "no access to that resource.";
        var unauthorizedAccessException = new UnauthorizedAccessException(errorMessage);
        var (next, context) = GenerateDelegateAndContextFromException(unauthorizedAccessException);

        var expectedResponseBody = JsonSerializer.Serialize(new ErrorsDto(
            new List<ErrorDto>
            {
            new ErrorDto(StatusCodes.Status403Forbidden, errorMessage)
            }));

        // Act
        await _exceptionMiddleware.InvokeAsync(context, next);

        // Assert
        using (new AssertionScope())
        {
            var responseBody = await DeserializeResponseBody(context);
            context.Response.ContentType.Should().Be(ContentType);
            context.Response.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
            responseBody.Should().BeEquivalentTo(expectedResponseBody.ToLower());
        }
    }

    [Fact]
    public async Task InvokeAsync_DelegateThrowsInputValidationSame400ErrorsException_Generates400()
    {
        // Arrange
        var errorMessage1 = "bad request";
        var errorMessage2 = "bad request2";

        var errors = new List<Error>()
        {
            new Error(StatusCodes.Status400BadRequest, errorMessage1),
            new Error(StatusCodes.Status400BadRequest, errorMessage2)
        };

        var inputValidationException = new InputValidationException(errors);
        var (next, context) = GenerateDelegateAndContextFromException(inputValidationException);

        var expectedResponseBody = JsonSerializer.Serialize(new ErrorsDto(
            new List<ErrorDto>
            {
            new ErrorDto(StatusCodes.Status400BadRequest, errorMessage1),
            new ErrorDto(StatusCodes.Status400BadRequest, errorMessage2)
            }));

        // Act
        await _exceptionMiddleware.InvokeAsync(context, next);

        // Assert
        using (new AssertionScope())
        {
            var responseBody = await DeserializeResponseBody(context);
            context.Response.ContentType.Should().Be(ContentType);
            context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            responseBody.Should().BeEquivalentTo(expectedResponseBody.ToLower());
        }
    }

    [Fact]
    public async Task InvokeAsync_DelegateThrowsInputValidationDifferentStatusErrorsException_Generates207()
    {
        // Arrange
        var errorMessage1 = "bad request";
        var errorMessage2 = "something not found";

        var errors = new List<Error>()
        {
            new Error(StatusCodes.Status400BadRequest, errorMessage1),
            new Error(StatusCodes.Status404NotFound, errorMessage2)
        };

        var inputValidationException = new InputValidationException(errors);
        var (next, context) = GenerateDelegateAndContextFromException(inputValidationException);

        var expectedResponseBody = JsonSerializer.Serialize(new ErrorsDto(
            new List<ErrorDto>
            {
            new ErrorDto(StatusCodes.Status400BadRequest, errorMessage1),
            new ErrorDto(StatusCodes.Status404NotFound, errorMessage2)
            }));

        // Act
        await _exceptionMiddleware.InvokeAsync(context, next);

        // Assert
        using (new AssertionScope())
        {
            var responseBody = await DeserializeResponseBody(context);
            context.Response.ContentType.Should().Be(ContentType);
            context.Response.StatusCode.Should().Be(StatusCodes.Status207MultiStatus);
            responseBody.Should().BeEquivalentTo(expectedResponseBody.ToLower());
        }
    }

    [Fact]
    public async Task InvokeAsync_DelegateThrowsException_Generates500()
    {
        // Arrange
        var errorMessage = "uh oh idk what to do :/";
        var exception = new Exception(errorMessage);
        var (next, context) = GenerateDelegateAndContextFromException(exception);

        var expectedResponseBody = JsonSerializer.Serialize(new ErrorsDto(
            new List<ErrorDto>
            {
            new ErrorDto(StatusCodes.Status500InternalServerError, "An unhandled exception occurred.")
            }));

        // Act
        await _exceptionMiddleware.InvokeAsync(context, next);

        // Assert
        using (new AssertionScope())
        {
            var responseBody = await DeserializeResponseBody(context);
            context.Response.ContentType.Should().Be(ContentType);
            context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            responseBody.Should().BeEquivalentTo(expectedResponseBody.ToLower());
        }
    }

    private (RequestDelegate, HttpContext) GenerateDelegateAndContextFromException(Exception exception)
    {
        HttpContext context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        RequestDelegate next = (context) => { throw exception; };

        return (next, context);
    }

    private async Task<string> DeserializeResponseBody(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using (var reader = new StreamReader(context.Response.Body))
        {
            var responseBody = await reader.ReadToEndAsync();

            return responseBody;
        }
    }
}
