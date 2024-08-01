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

public class DeleteAdminEndpointTests
{
    private readonly ILogger<IdentityController> _loggerMock;
    private readonly IRemoveAccountService _removeAccountServiceMock;
    private readonly IClaimsService _claimsServiceMock;
    private readonly IRegistrationService _registrationServiceMock;
    private readonly IAuthTokenService _authTokenServiceMock;
    private readonly IMapper _mapper;
    private readonly IdentityController _identityController;

    public DeleteAdminEndpointTests()
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
    public async Task DeleteAdminAsync_UsernamesMatchAndValidPassword_ReturnsOk()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var username = "lukosparta123";
        var password = "rollo123";

        var claimsEmail = "lukewwaterhouse@hotmail.com";
        var claimsRole = "Admin";

        var claims = new Claims(username, claimsEmail, claimsRole, DateTime.UtcNow);
        _claimsServiceMock.GetClaimsFromUser(Arg.Any<ClaimsPrincipal>()).Returns(claims);

        // Act
        var result = await _identityController.DeleteAdminAsync(username, password, null, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkResult>();
            await _removeAccountServiceMock.Received(1).DeleteAdminAsAdminAsync(username, password);
            await _removeAccountServiceMock.DidNotReceiveWithAnyArgs().DeleteAdminAsAdminWithKeyAsync(Arg.Any<string>(), Arg.Any<string>());
        }
    }

    [Fact]
    public async Task DeleteAdminAsync_UsernamesMatchAndNoPassword_ThrowsException()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var username = "lukosparta123";
        string? password = null;

        var claimsEmail = "lukewwaterhouse@hotmail.com";
        var claimsRole = "Admin";

        var claims = new Claims(username, claimsEmail, claimsRole, DateTime.UtcNow);
        _claimsServiceMock.GetClaimsFromUser(Arg.Any<ClaimsPrincipal>()).Returns(claims);

        //Act
        Exception exception = await Record.ExceptionAsync(async () =>
        {
            var result = await _identityController.DeleteAdminAsync(username, password, null, cancellationToken);
        });

        // Assert
        using (new AssertionScope())
        {
            exception.Should().BeOfType<UnauthorizedAccessException>();
            exception.Message.Should().Be("You must provide a password when deleting your own admin account");
            await _removeAccountServiceMock.DidNotReceiveWithAnyArgs().DeleteAdminAsAdminAsync(Arg.Any<string>(), Arg.Any<string>());
            await _removeAccountServiceMock.DidNotReceiveWithAnyArgs().DeleteAdminAsAdminWithKeyAsync(Arg.Any<string>(), Arg.Any<string>());
        }
    }

    [Fact]
    public async Task DeleteAdminAsync_UsernameMismatchAndValidKey_ReturnsOk()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var formUsername = "lukosparta123";
        var password = "rollo123";
        var adminKey = "123";

        var claimsUsername = "kratos1998";
        var claimsEmail = "lukewwaterhouse@hotmail.com";
        var claimsRole = "Admin";

        var claims = new Claims(claimsUsername, claimsEmail, claimsRole, DateTime.UtcNow);
        _claimsServiceMock.GetClaimsFromUser(Arg.Any<ClaimsPrincipal>()).Returns(claims);

        // Act
        var result = await _identityController.DeleteAdminAsync(formUsername, password, adminKey, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkResult>();
            await _removeAccountServiceMock.DidNotReceiveWithAnyArgs().DeleteAdminAsAdminAsync(Arg.Any<string>(), Arg.Any<string>());
            await _removeAccountServiceMock.Received(1).DeleteAdminAsAdminWithKeyAsync(formUsername, adminKey);
        }
    }

    [Fact]
    public async Task DeleteAdminAsync_UsernameMismatchAndNoKey_ThrowsException()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var formUsername = "lukosparta123";

        var claimsUsername = "kratos1998";
        var claimsEmail = "lukewwaterhouse@hotmail.com";
        var claimsRole = "Admin";

        var claims = new Claims(claimsUsername, claimsEmail, claimsRole, DateTime.UtcNow);
        _claimsServiceMock.GetClaimsFromUser(Arg.Any<ClaimsPrincipal>()).Returns(claims);

        //Act
        Exception exception = await Record.ExceptionAsync(async () =>
        {
            var result = await _identityController.DeleteAdminAsync(formUsername, null, null, cancellationToken);
        });

        // Assert
        using (new AssertionScope())
        {
            exception.Should().BeOfType<UnauthorizedAccessException>();
            exception.Message.Should().Be($"You must provide an admin key when deleting another admin account: {formUsername}");
            await _removeAccountServiceMock.DidNotReceiveWithAnyArgs().DeleteAdminAsAdminAsync(Arg.Any<string>(), Arg.Any<string>());
            await _removeAccountServiceMock.DidNotReceiveWithAnyArgs().DeleteAdminAsAdminWithKeyAsync(Arg.Any<string>(), Arg.Any<string>());
        }
    }
}
