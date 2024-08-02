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

public class DeleteUserByNameEndpointTests
{
    private readonly ILogger<IdentityController> _loggerMock;
    private readonly IRemoveAccountService _removeAccountServiceMock;
    private readonly IClaimsService _claimsServiceMock;
    private readonly IRegistrationService _registrationServiceMock;
    private readonly IAuthTokenService _authTokenServiceMock;
    private readonly IMapper _mapper;
    private readonly IdentityController _identityController;

    public DeleteUserByNameEndpointTests()
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

    [Theory]
    [InlineData("adminKey123")]
    [InlineData(null)]
    public async Task DeleteUserAsAdminAsync_ValidUsername_ReturnsOk(string? adminKey)
    {
        // Arrange
        var username = "lukosparta123";
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _identityController.DeleteUserByNameAsync(username, adminKey, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkResult>();
            await _removeAccountServiceMock.Received(1).DeleteUserByNameAsync(username, adminKey);
        }
    }
}
