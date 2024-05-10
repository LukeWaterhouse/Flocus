using Flocus.Domain.Interfacesl;
using Flocus.Domain.Models;
using Flocus.Identity.Services;
using Flocus.Identity.Tests.Services.Identity.IdentityTestHelpers;
using Flocus.Repository.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Text;
using Xunit;
using BC = BCrypt.Net.BCrypt;


namespace Flocus.Identity.Tests.Services.Identity;

public class GetAuthTokenAsyncTests
{
    private readonly IRepositoryService _repositoryServiceMock;
    private readonly IConfiguration _configurationMock;
    private readonly IdentityService _identityService;
    private readonly string _signingKey;

    public GetAuthTokenAsyncTests()
    {
        _signingKey = GenerateRandomId(200);
        _repositoryServiceMock = Substitute.For<IRepositoryService>();
        _configurationMock = Substitute.For<IConfiguration>();
        _configurationMock.GetSection("AppSettings").Returns(ConfigTestHelper.GenerateConfigSection(_signingKey, "adminKeyValue"));
        _identityService = new IdentityService(_repositoryServiceMock, _configurationMock);
    }

    [Fact]
    public async Task GetAuthAsync_WithValidCredentials_ReturnsValidToken()
    {
        //Arrange
        var username = "luke";
        var password = "rollo123";
        var passwordHash = BC.HashPassword(password);

        _repositoryServiceMock.GetUserAsync(username).Returns(new User
        {
            ClientId = "clientId",
            Username = username,
            CreatedAt = DateTime.UtcNow,
            IsAdmin = false,
            PasswordHash = passwordHash
        });

        //Act
        var token = await _identityService.GetAuthTokenAsync(username, password);

        //Assert
        token.Should().NotBeNull();
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwt = jwtHandler.ReadJwtToken(token);
        var claims = jwt.Claims.ToList();

        claims[0].Type.Should().Be("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        claims[0].Value.Should().Be(username);

        claims[1].Type.Should().Be("exp");
        UnixTimeStampStringToDateTime(claims[1].Value).Should().BeCloseTo(DateTime.UtcNow.AddDays(1), TimeSpan.FromMinutes(10));
    }

    [Fact]
    public async Task GetAuthAsync_WithIncorrectPassword_ThrowsCorrectException()
    {
        //Arrange
        var username = "luke";
        var correctPassword = "rollo123";
        var incorrectPassword = "rollo1234";
        var passwordHash = BC.HashPassword(correctPassword);

        _repositoryServiceMock.GetUserAsync(username).Returns(new User
        {
            ClientId = "clientId",
            Username = username,
            CreatedAt = DateTime.UtcNow,
            IsAdmin = false,
            PasswordHash = passwordHash
        });

        //Act
        Exception exception = await Record.ExceptionAsync(async () =>
        {
            var token = await _identityService.GetAuthTokenAsync(username, incorrectPassword);
        });

        //Assert
        exception.Should().BeOfType<AuthenticationException>();
        exception.Message.Should().Be("Invalid username and password combination");
    }

    [Fact]
    public async Task GetAuthAsync_NonExistingUser_ThrowsCorrectException()
    {
        //Arrange
        var username = "luke";
        var password = "rollo123";
        var passwordHash = BC.HashPassword(password);

        _repositoryServiceMock.GetUserAsync(username).Throws(new RecordNotFoundException("user not found"));

        //Act
        Exception exception = await Record.ExceptionAsync(async () =>
        {
            var token = await _identityService.GetAuthTokenAsync(username, password);
        });

        //Assert
        exception.Should().BeOfType<AuthenticationException>();
        exception.Message.Should().Be("Invalid username and password combination");
    }

    [Fact]
    public async Task GetAuthAsync_SigningKeyNotSet_ThrowsCorrectException()
    {
        //Arrange
        var username = "luke";
        var password = "rollo123";
        var passwordHash = BC.HashPassword(password);

        _repositoryServiceMock.GetUserAsync(username).Returns(new User
        {
            ClientId = "clientId",
            Username = username,
            CreatedAt = DateTime.UtcNow,
            IsAdmin = false,
            PasswordHash = passwordHash
        });

        var configurationMock = Substitute.For<IConfiguration>();
        configurationMock.GetSection("AppSettings").Returns(ConfigTestHelper.GenerateConfigSection(null, "adminKeyValue"));
        var identityService = new IdentityService(_repositoryServiceMock, configurationMock);

        //Act
        Exception exception = await Record.ExceptionAsync(async () =>
        {
            var token = await identityService.GetAuthTokenAsync(username, password);
        });

        //Assert
        exception.Should().BeOfType<Exception>();
        exception.Message.Should().Be("invalid signing key from config");
    }


    private string GenerateRandomId(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var idBuilder = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            idBuilder.Append(chars[random.Next(chars.Length)]);
        }

        return idBuilder.ToString();
    }

    private DateTime UnixTimeStampStringToDateTime(string unixTimestampString)
    {
        if (!long.TryParse(unixTimestampString, out long unixTimestamp))
        {
            throw new ArgumentException("Invalid Unix timestamp string", nameof(unixTimestampString));
        }

        DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return unixEpoch.AddSeconds(unixTimestamp);
    }
}
