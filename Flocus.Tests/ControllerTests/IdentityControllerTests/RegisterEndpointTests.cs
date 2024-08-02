using AutoMapper;
using Flocus.Controllers;
using Flocus.Identity.Interfaces;
using Flocus.Identity.Interfaces.AuthTokenInterfaces;
using Flocus.Identity.Interfaces.RegisterInterfaces;
using Flocus.Identity.Interfaces.RemoveAccountInterfaces;
using Flocus.Identity.Models;
using Flocus.Mapping;
using Flocus.Models.Requests;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Flocus.Tests.ControllerTests.IdentityControllerTests;

public sealed class RegisterEndpointTests
{
    private readonly ILogger<IdentityController> _loggerMock;
    private readonly IRemoveAccountService _removeAccountServiceMock;
    private readonly IClaimsService _claimsServiceMock;
    private readonly IRegistrationService _registrationServiceMock;
    private readonly IAuthTokenService _authTokenServiceMock;
    private readonly IMapper _mapper;
    private readonly IdentityController _identityController;

    public RegisterEndpointTests()
    {
        _loggerMock = Substitute.For<ILogger<IdentityController>>();
        _removeAccountServiceMock = Substitute.For<IRemoveAccountService>();
        _claimsServiceMock = Substitute.For<IClaimsService>();
        _registrationServiceMock = Substitute.For<IRegistrationService>();
        _authTokenServiceMock = Substitute.For<IAuthTokenService>();

        var mappingConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(RegistrationModelMappingProfile));
        });
        _mapper = mappingConfig.CreateMapper();

        _identityController = new IdentityController(
            _loggerMock,
            _mapper,
            _claimsServiceMock,
            _registrationServiceMock,
            _authTokenServiceMock,
            _removeAccountServiceMock);
    }

    [Fact]
    public async Task RegisterAsync_ValidNonAdminRequest_CallsIdentityServiceReturnsOk()
    {
        // Arrange
        var registerRequest = new RegisterRequestDto("luke", "rollo123", "luke@hotmail.com", false, null);

        // Act
        var result = await _identityController.RegisterAsync(registerRequest);

        // Assert
        var expectedCallParameter = new RegistrationModel(
            registerRequest.Username,
            registerRequest.Password,
            registerRequest.EmailAddress,
            registerRequest.IsAdmin,
            registerRequest.Key);

        using (new AssertionScope())
        {
            result.Should().BeOfType<OkResult>();
            await _registrationServiceMock.Received().RegisterAsync(expectedCallParameter);
        }
    }

    [Fact]
    public async Task RegisterAsync_ValidAdminRequest_CallsIdentityServiceReturnsOk()
    {
        // Arrange
        var registerRequest = new RegisterRequestDto("luke", "rollo123", "luke@hotmail.com", true, "123");

        // Act
        var result = await _identityController.RegisterAsync(registerRequest);

        // Assert
        var expectedCallParameter = new RegistrationModel(
            registerRequest.Username,
            registerRequest.Password,
            registerRequest.EmailAddress,
            registerRequest.IsAdmin,
            registerRequest.Key);

        using (new AssertionScope())
        {
            result.Should().BeOfType<OkResult>();
            await _registrationServiceMock.Received().RegisterAsync(expectedCallParameter);
        }
    }
}
