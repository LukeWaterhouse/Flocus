using AutoMapper;
using Flocus.Mapping;
using Flocus.Middleware;
using Flocus.Models.Errors;
using Flocus.Repository.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Security.Authentication;
using System.Text.Json;
using Xunit;

namespace Flocus.Tests.Middleware;


//TODO: Add assertion scopes
public class ExceptionMiddlewareTests
{
    private readonly ILogger<ExceptionMiddleware> _loggerMock;
    private readonly ExceptionMiddleware _exceptionMiddleware;
    private readonly IMapper _mapper;

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
        var duplicateRecordException = new DuplicateRecordException("duplicate record found.");
        HttpContext context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        RequestDelegate next = (context) => { throw duplicateRecordException; };

        var expectedResponseBody = JsonSerializer.Serialize(new ErrorsDto(
            new List<ErrorDto>
            {
            new ErrorDto(409, "duplicate record found.")
            }));

        // Act
        await _exceptionMiddleware.InvokeAsync(context, next);

        // Assert
        var responseBody = await DeserializeResponseBody(context);
        context.Response.ContentType.Should().Be("application/json; charset=utf-8");
        responseBody.Should().BeEquivalentTo(expectedResponseBody.ToLower());
    }

    [Fact]
    public async Task InvokeAsync_DelegateThrowsRecordNotFoundException_Generates404()
    {
        // Arrange
        var recordNotFoundException = new RecordNotFoundException("record not found.");
        HttpContext context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        RequestDelegate next = (context) => { throw recordNotFoundException; };

        var expectedResponseBody = JsonSerializer.Serialize(new ErrorsDto(
            new List<ErrorDto>
            {
            new ErrorDto(404, "record not found.")
            }));

        // Act
        await _exceptionMiddleware.InvokeAsync(context, next);

        // Assert
        var responseBody = await DeserializeResponseBody(context);
        context.Response.ContentType.Should().Be("application/json; charset=utf-8");
        responseBody.Should().BeEquivalentTo(expectedResponseBody.ToLower());
    }

    [Fact]
    public async Task InvokeAsync_DelegateThrowsAuthenticationException_Generates401()
    {
        // Arrange
        var authenticationException = new AuthenticationException("password username combo incorrect");
        HttpContext context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        RequestDelegate next = (context) => { throw authenticationException; };

        var expectedResponseBody = JsonSerializer.Serialize(new ErrorsDto(
            new List<ErrorDto>
            {
            new ErrorDto(401, "password username combo incorrect")
            }));

        // Act
        await _exceptionMiddleware.InvokeAsync(context, next);

        // Assert
        var responseBody = await DeserializeResponseBody(context);
        context.Response.ContentType.Should().Be("application/json; charset=utf-8");
        responseBody.Should().BeEquivalentTo(expectedResponseBody.ToLower());
    }

    [Fact]
    public async Task InvokeAsync_DelegateThrowsException_Generates500()
    {
        // Arrange
        var exception = new Exception("uh oh idk what to do :/");
        HttpContext context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        RequestDelegate next = (context) => { throw exception; };

        var expectedResponseBody = JsonSerializer.Serialize(new ErrorsDto(
            new List<ErrorDto>
            {
            new ErrorDto(500, "An unhandled exception occurred.")
            }));

        // Act
        await _exceptionMiddleware.InvokeAsync(context, next);

        // Assert
        var responseBody = await DeserializeResponseBody(context);
        context.Response.ContentType.Should().Be("application/json; charset=utf-8");
        responseBody.Should().BeEquivalentTo(expectedResponseBody.ToLower());
    }

    public async Task<string> DeserializeResponseBody(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using (var reader = new StreamReader(context.Response.Body))
        {
            var responseBody = await reader.ReadToEndAsync();

            return responseBody;
        }
    }
}
