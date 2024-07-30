using AutoMapper;
using Flocus.Controllers;
using Flocus.Identity.Interfaces;
using Flocus.Identity.Interfaces.AuthTokenInterfaces;
using Flocus.Identity.Interfaces.RegisterInterfaces;
using Flocus.Identity.Models;
using Flocus.Mapping;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Security.Claims;
using Xunit;

namespace Flocus.Tests.ControllerTests.IdentityControllerTests;

public class DeleteUserAsUserEndpointTests
{
    private readonly ILogger<IdentityController> _loggerMock;
    private readonly IRemoveAccountService _removeAccountServiceMock;
    private readonly IClaimsService _claimsServiceMock;
    private readonly IRegistrationService _registrationServiceMock;
    private readonly IAuthTokenService _authTokenServiceMock;
    private readonly IMapper _mapper;
    private readonly IdentityController _identityController;

    public DeleteUserAsUserEndpointTests()
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
    public async Task DeleteUserAsUserAsync_ValidPasswordAndClaims_ReturnsOk()
    {
        // Arrange
        var username = "lukosparta123";
        var email = "lukewwaterhouse@hotmail.com";
        var role = "Admin";
        var password = "rollo123";
        var cancellationToken = CancellationToken.None;

        var claims = new Claims(username, email, role, DateTime.UtcNow);
        _claimsServiceMock.GetClaimsFromUser(Arg.Any<ClaimsPrincipal>()).Returns(claims);

        // Act
        var result = await _identityController.DeleteUserAsUserAsync(password, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkResult>();
            await _removeAccountServiceMock.Received(1).DeleteUserAsUser(username, password);
        }
    }
}
