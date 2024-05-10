using Flocus.Controllers;
using Flocus.Identity.Interfaces;
using Flocus.Models.Errors;
using Flocus.Models.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Flocus.Tests.Controllers.IdentityControllerTests;

public class RegisterEndpointTests
{
    private readonly ILogger<IdentityController> _loggerMock;
    private readonly IIdentityService _identityServiceMock;
    private readonly IdentityController _identityController;

    public RegisterEndpointTests()
    {
        _loggerMock = Substitute.For<ILogger<IdentityController>>();
        _identityServiceMock = Substitute.For<IIdentityService>();
        _identityController = new IdentityController(_loggerMock, _identityServiceMock);
    }

    [Fact]
    public async Task RegisterAsync_ValidNonAdminRequest_CallsIdentityServiceReturnsOk()
    {
        //Arrange
        var registerRequest = new RegisterRequestDto("luke", "rollo123", "luke@hotmail.com", false, null);

        //Act
        var result = await _identityController.RegisterAsync(registerRequest, CancellationToken.None);

        //Assert
        result.Should().BeOfType<OkResult>();
        await _identityServiceMock.Received().RegisterAsync(
            registerRequest.username,
            registerRequest.password,
            registerRequest.emailAddress,
            registerRequest.isAdmin,
            registerRequest.key
            );
    }

    [Fact]
    public async Task RegisterAsync_ValidAdminRequest_CallsIdentityServiceReturnsOk()
    {
        //Arrange
        var registerRequest = new RegisterRequestDto("luke", "rollo123", "luke@hotmail.com", true, "123");

        //Act
        var result = await _identityController.RegisterAsync(registerRequest, CancellationToken.None);

        //Assert
        result.Should().BeOfType<OkResult>();
        await _identityServiceMock.Received().RegisterAsync(
            registerRequest.username,
            registerRequest.password,
            registerRequest.emailAddress,
            registerRequest.isAdmin,
            registerRequest.key);
    }

    [Fact]
    public async Task RegisterAsync_AdminRequestNoKey_DoesNotCallIdentityServiceReturnsBadRequest()
    {
        //Arrange
        var registerRequest = new RegisterRequestDto("luke", "rollo123", "luke@hotmail.com", true, null);

        //Act
        var result = await _identityController.RegisterAsync(registerRequest, CancellationToken.None);

        //Assert
        var expectedErrors = new ErrorsDto(new List<ErrorDto> { new ErrorDto(400, "Must provide key when creating admin") });

        result.Should().BeOfType<BadRequestObjectResult>();
        var body = ((BadRequestObjectResult)result).Value;
        body.Should().BeEquivalentTo(expectedErrors);
        result.Should().BeOfType<BadRequestObjectResult>();
        _identityServiceMock.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public async Task RegisterAsync_InvalidRequestModel_DoesNotCallIdentityServiceReturnsBadRequestWithErrors()
    {
        //Arrange
        var registerRequest = new RegisterRequestDto("luke", "rollo123", "luke@hotmail.com", true, "asd");
        var identityController = new IdentityController(_loggerMock, _identityServiceMock);
        identityController.ModelState.AddModelError("validationKey", "validationError");
        identityController.ModelState.AddModelError("validationKey2", "validationError2");


        //Act
        var result = await identityController.RegisterAsync(registerRequest, CancellationToken.None);

        //Assert
        var expectedErrors = new ErrorsDto(new List<ErrorDto> {
            new ErrorDto(400, "validationError"),
            new ErrorDto(400, "validationError2")
        });

        result.Should().BeOfType<BadRequestObjectResult>();
        var body = ((BadRequestObjectResult)result).Value;
        body.Should().BeEquivalentTo(expectedErrors);
        result.Should().BeOfType<BadRequestObjectResult>();
        _identityServiceMock.ReceivedCalls().Should().BeEmpty();
    }
}
