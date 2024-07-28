using Flocus.Domain.Interfaces;
using Flocus.Identity.Interfaces;
using Flocus.Identity.Models;
using Flocus.Identity.Services.RemoveAccountServices;
using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using System.Security.Authentication;
using Xunit;
using BC = BCrypt.Net.BCrypt;


namespace Flocus.Identity.Tests.Services.Identity;

public class RegisterAsyncTests
{
    private readonly IUserRepositoryService _repositoryServiceMock;
    private readonly IRegistrationValidationService _registerValidationServiceMock;
    private readonly IdentitySettings _identitySettings;
    private readonly RemoveAccountService _identityService;

    public RegisterAsyncTests()
    {
        _repositoryServiceMock = Substitute.For<IUserRepositoryService>();
        _registerValidationServiceMock = Substitute.For<IRegistrationValidationService>();
        _identitySettings = new IdentitySettings("signingKey", "issuer", "audience", "adminKey");
        _identityService = new IdentityService(_repositoryServiceMock, _registerValidationServiceMock, _identitySettings);
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

        var registrationModel = new RegistrationModel(
            username,
            password,
            email,
            isAdmin,
            key);

        //Act
        await _identityService.RegisterAsync(registrationModel);

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

        var registrationModel = new RegistrationModel(
            username,
            password,
            email,
            isAdmin,
            key);

        //Act
        await _identityService.RegisterAsync(registrationModel);

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

        var registrationModel = new RegistrationModel(
            username,
            password,
            email,
            isAdmin,
            key);

        //Act
        Exception exception = await Record.ExceptionAsync(async () =>
        {
            await _identityService.RegisterAsync(registrationModel);
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
