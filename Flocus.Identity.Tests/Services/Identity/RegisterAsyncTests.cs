using Flocus.Domain.Interfacesl;
using Flocus.Identity.Models;
using Flocus.Identity.Services;
using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using System.Security.Authentication;
using Xunit;
using BC = BCrypt.Net.BCrypt;


namespace Flocus.Identity.Tests.Services.Identity;

public class RegisterAsyncTests
{
    private readonly IRepositoryService _repositoryServiceMock;
    private readonly IdentitySettings _identitySettings;
    private readonly IdentityService _identityService;

    public RegisterAsyncTests()
    {
        _repositoryServiceMock = Substitute.For<IRepositoryService>();
        _identitySettings = new IdentitySettings("signingKey", "issuer", "audience", "adminKey");
        _identityService = new IdentityService(_repositoryServiceMock, _identitySettings);
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
        var key = "adminKey";

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
        using (new AssertionScope())
        {
            exception.Should().BeOfType<AuthenticationException>();
            exception.Message.Should().Be("Admin key is not correct");
            _repositoryServiceMock.ReceivedCalls().Should().BeEmpty();
        }
    }

    bool VerifyPassword(string password, string input)
    {
        return BC.Verify(password, input);
    }
}
