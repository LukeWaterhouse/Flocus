using AutoMapper;
using Flocus.Controllers;
using Flocus.Identity.Interfaces;
using Flocus.Identity.Interfaces.AuthTokenInterfaces;
using Flocus.Identity.Interfaces.RegisterInterfaces;
using Flocus.Identity.Interfaces.RemoveAccountInterfaces;
using Flocus.Mapping;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Flocus.Tests.ControllerTests.IdentityControllerTests;

public sealed class GetTokenEndpointTests
{
    public readonly ILogger<IdentityController> _loggerMock;
    public readonly IRemoveAccountService _removeAccountServiceMock;
    public readonly IClaimsService _claimsServiceMock;
    public readonly IRegistrationService _registrationServiceMock;
    public readonly IAuthTokenService _authTokenServiceMock;
    public readonly IMapper _mapper;
    public readonly IdentityController _identityController;

    public GetTokenEndpointTests()
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
    public async Task GetTokenAsync_ValidRequest_ReturnsOkWithToken()
    {
        // Arrange
        var username = "luke";
        var password = "rollo123";

        _authTokenServiceMock.GetAuthTokenAsync(username, password).Returns(Task.FromResult("token"));

        // Act
        var result = await _identityController.GetTokenAsync(username, password);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            var body = ((OkObjectResult)result).Value!;
            body.Should().BeEquivalentTo("token");
        }
    }
}
