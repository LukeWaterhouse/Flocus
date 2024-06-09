using Flocus.Domain.Interfacesl;
using Flocus.Identity.Services;
using Flocus.Identity.Tests.Services.Identity.IdentityTestHelpers;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using System.Security.Authentication;
using Xunit;
using BC = BCrypt.Net.BCrypt;


namespace Flocus.Identity.Tests.Services.Identity;

public class RegisterAsyncTests
{
    private readonly IRepositoryService _repositoryServiceMock;
    private readonly IConfiguration _configurationMock;
    private readonly IdentityService _identityService;

    public RegisterAsyncTests()
    {
        _repositoryServiceMock = Substitute.For<IRepositoryService>();
        _configurationMock = Substitute.For<IConfiguration>();
        _configurationMock.GetSection("AppSettings").Returns(ConfigTestHelper.GenerateConfigSection("signingKeyValue", "adminKeyValue"));
        _identityService = new IdentityService(_repositoryServiceMock, _configurationMock);
    }

    [Fact]
    public async Task RegisterAsync_WithValidParameters_CallsRepositoryService()
    {
        //Arrange
        var username = "luke";
        var password = "hashedPassword";
        var isAdmin = false;
        var email = "luke@hotmail.com";
        var key = "key";

        _repositoryServiceMock.CreateDbUserAsync(username, Arg.Is<string>(hash => VerifyPassword(password, hash)), email, isAdmin).Returns(true);

        //Act
        await _identityService.RegisterAsync(username, password, email, isAdmin, key);

        //Assert
        await _repositoryServiceMock.Received().CreateDbUserAsync(
            username,
            Arg.Is<string>(hash => VerifyPassword(password, hash)),
            email,
            isAdmin);
    }

    [Fact]
    public async Task RegisterAsync_WithValidAdmin_CallsRepositoryService()
    {
        //Arrange
        var username = "luke";
        var password = "hashedPassword";
        var isAdmin = true;
        var email = "luke@hotmail.com";
        var key = "adminKeyValue";

        _repositoryServiceMock.CreateDbUserAsync(username, Arg.Is<string>(hash => VerifyPassword(password, hash)), email, isAdmin).Returns(true);

        //Act
        await _identityService.RegisterAsync(username, password, email, isAdmin, key);

        //Assert
        await _repositoryServiceMock.Received().CreateDbUserAsync(
            username,
            Arg.Is<string>(hash => VerifyPassword(password, hash)),
            email,
            isAdmin);
    }

    [Fact]
    public async Task RegisterAsync_WithIncorrectAdminKey_ThrowsAuthException()
    {
        //Arrange
        var username = "luke";
        var password = "hashedPassword";
        var isAdmin = true;
        var email = "luke@hotmail.com";
        var key = "incorrectValue";

        //Act
        Exception exception = await Record.ExceptionAsync(async () =>
        {
            await _identityService.RegisterAsync(username, password, email, isAdmin, key);
        });

        //Assert
        exception.Should().BeOfType<AuthenticationException>();
        exception.Message.Should().Be("Key was incorrect.");
        _repositoryServiceMock.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public async Task RegisterAsync_CreateDbUSerReturnsFalse_ThrowsException()
    {
        //Arrange
        var username = "luke";
        var password = "hashedPassword";
        var isAdmin = true;
        var email = "luke@hotmail.com";
        var key = "adminKeyValue";

        _repositoryServiceMock.CreateDbUserAsync(username, Arg.Is<string>(hash => VerifyPassword(password, hash)), email, isAdmin).Returns(false);

        //Act
        Exception exception = await Record.ExceptionAsync(async () =>
        {
            await _identityService.RegisterAsync(username, password, email, isAdmin, key);
        });

        //Assert
        exception.Should().BeOfType<Exception>();
        exception.Message.Should().Be("There was an error when creating the user.");
        await _repositoryServiceMock.Received().CreateDbUserAsync(
                    username,
                    Arg.Is<string>(hash => VerifyPassword(password, hash)),
                    email,
                    isAdmin);
    }

    bool VerifyPassword(string password, string input)
    {
        return BC.Verify(password, input);
    }
}
