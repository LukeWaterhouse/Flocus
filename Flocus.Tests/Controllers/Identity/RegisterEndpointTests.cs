using AutoMapper;
using Flocus.Controllers;
using Flocus.Identity.Interfaces;
using Flocus.Identity.Interfaces.AuthTokenInterfaces;
using Flocus.Identity.Interfaces.RegisterInterfaces;
using Flocus.Identity.Models;
using Flocus.Mapping;
using Flocus.Models.Requests;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Flocus.Tests.Controllers.Identity;

public class RegisterEndpointTests
{
    private readonly ILogger<IdentityController> _loggerMock;
    private readonly IRemoveAccountService _identityServiceMock;
    private readonly IClaimsService _claimsServiceMock;
    private readonly IRegistrationService _registrationServiceMock;
    private readonly IAuthTokenService _authTokenService;
    private readonly IMapper _mapper;
    private readonly IdentityController _identityController;

    public RegisterEndpointTests()
    {
        _loggerMock = Substitute.For<ILogger<IdentityController>>();
        _identityServiceMock = Substitute.For<IRemoveAccountService>();
        _claimsServiceMock = Substitute.For<IClaimsService>();
        _registrationServiceMock = Substitute.For<IRegistrationService>();
        _authTokenService = Substitute.For<IAuthTokenService>();

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
            _authTokenService,
            _identityServiceMock);
    }

    [Fact]
    public async Task RegisterAsync_ValidNonAdminRequest_CallsIdentityServiceReturnsOk()
    {
        //Arrange
        var registerRequest = new RegisterRequestDto("luke", "rollo123", "luke@hotmail.com", false, null);

        //Act
        var result = await _identityController.RegisterAsync(registerRequest, CancellationToken.None);

        //Assert
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
        //Arrange
        var registerRequest = new RegisterRequestDto("luke", "rollo123", "luke@hotmail.com", true, "123");

        //Act
        var result = await _identityController.RegisterAsync(registerRequest, CancellationToken.None);

        //Assert
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
