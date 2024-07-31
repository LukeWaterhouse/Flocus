using Flocus.Domain.Common;
using Flocus.Domain.Interfaces;
using Flocus.Domain.Models;
using Flocus.Identity.Interfaces.AuthTokenInterfaces;
using Flocus.Identity.Models;
using Flocus.Identity.Services.AuthTokenServices;
using Flocus.Repository.Exceptions;
using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using Xunit;
using BC = BCrypt.Net.BCrypt;


namespace Flocus.Identity.Tests.Services.AuthTokenServicesTests;

public class AuthTokenServiceTests
{
    private readonly IUserRepositoryService _userRepositoryServiceMock;
    private readonly IdentitySettings _identitySettings;

    private readonly IAuthTokenService _authTokenService;

    public AuthTokenServiceTests()
    {
        _identitySettings = new IdentitySettings(
            "signingKey-13123190283jh19028n12983n190238n190283n109283n109283n109283n09812n309182n3109283n098n",
            "issuer",
            "audience",
            "adminKey");
        _userRepositoryServiceMock = Substitute.For<IUserRepositoryService>();

        _authTokenService = new AuthTokenService(_userRepositoryServiceMock, _identitySettings);
    }

    [Fact]
    public async Task GetAuthAsync_WithValidCredentials_ReturnsValidToken()
    {
        // Arrange
        var username = "luke";
        var password = "rollo123";
        var email = "luklerollo@hotmail.co.uk";
        var passwordHash = BC.HashPassword(password);

        _userRepositoryServiceMock.GetUserAsync(username).Returns(
            new User(
                "clientId",
                email,
                DateTime.UtcNow,
                username,
                false,
                passwordHash));

        // Act
        var token = await _authTokenService.GetAuthTokenAsync(username, password);

        // Assert
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwt = jwtHandler.ReadJwtToken(token);
        var claims = jwt.Claims.ToList();

        using (new AssertionScope())
        {
            token.Should().NotBeNull();
            claims[0].Type.Should().Be("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
            claims[0].Value.Should().Be(username);

            claims[1].Type.Should().Be("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
            claims[1].Value.Should().Be(email);

            claims[2].Type.Should().Be("http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
            claims[2].Value.Should().Be("User");

            claims[3].Type.Should().Be("exp");
            Utilities.UnixTimeStampStringToDateTime(claims[3].Value).Should().BeCloseTo(DateTime.UtcNow.AddDays(1), TimeSpan.FromMinutes(1));
        }
    }

    [Fact]
    public async Task GetAuthAsync_WithIncorrectPassword_ThrowsCorrectException()
    {
        // Arrange
        var username = "luke";
        var correctPassword = "rollo123";
        var incorrectPassword = "rollo1234";
        var passwordHash = BC.HashPassword(correctPassword);

        _userRepositoryServiceMock.GetUserAsync(username).Returns(
            new User(
                "clientId",
                "lukerollo@hotmail.co.uk",
                DateTime.UtcNow,
                username,
                false,
                passwordHash));

        // Act
        Exception exception = await Record.ExceptionAsync(async () =>
        {
            var token = await _authTokenService.GetAuthTokenAsync(username, incorrectPassword);
        });

        //Assert
        using (new AssertionScope())
        {
            exception.Should().BeOfType<AuthenticationException>();
            exception.Message.Should().Be("Incorrect username and password combination");
        }
    }

    [Fact]
    public async Task GetAuthAsync_NonExistingUser_ThrowsCorrectException()
    {
        // Arrange
        var username = "luke";
        var password = "rollo123";
        var passwordHash = BC.HashPassword(password);

        _userRepositoryServiceMock.GetUserAsync(username).Throws(new RecordNotFoundException("user not found"));

        // Act
        Exception exception = await Record.ExceptionAsync(async () =>
        {
            var token = await _authTokenService.GetAuthTokenAsync(username, password);
        });

        // Assert
        using (new AssertionScope())
        {
            exception.Should().BeOfType<AuthenticationException>();
            exception.Message.Should().Be("Incorrect username and password combination");
        }
    }
}
