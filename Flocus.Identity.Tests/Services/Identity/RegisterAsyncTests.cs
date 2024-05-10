using Flocus.Domain.Interfacesl;
using Flocus.Identity.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using System.Security.Authentication;
using System.Text;
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
        _configurationMock.GetSection("AppSettings").Returns(GenerateConfigSection());
        _identityService = new IdentityService(_repositoryServiceMock, _configurationMock);
    }

    [Fact]
    public async Task RegisterAsync_WithValidParameters_CallsRepositoryService()
    {
        //Arrange

        var username = "luke";
        var password = "hashedPassword";
        var isAdmin = false;
        var key = "key";

        _repositoryServiceMock.CreateDbUserAsync(username, Arg.Is<string>(hash => VerifyPassword(password, hash)), isAdmin).Returns(true);

        //Act
        await _identityService.RegisterAsync(username, password, isAdmin, key);

        //Assert
        await _repositoryServiceMock.Received().CreateDbUserAsync(
            username,
            Arg.Is<string>(hash => VerifyPassword(password, hash)),
            isAdmin);
    }

    [Fact]
    public async Task RegisterAsync_WithValidAdmin_CallsRepositoryService()
    {
        //Arrange

        var username = "luke";
        var password = "hashedPassword";
        var isAdmin = true;
        var key = "adminKeyValue";

        _repositoryServiceMock.CreateDbUserAsync(username, Arg.Is<string>(hash => VerifyPassword(password, hash)), isAdmin).Returns(true);

        //Act
        await _identityService.RegisterAsync(username, password, isAdmin, key);

        //Assert
        await _repositoryServiceMock.Received().CreateDbUserAsync(
            username,
            Arg.Is<string>(hash => VerifyPassword(password, hash)),
            isAdmin);
    }

    [Fact]
    public async Task RegisterAsync_WithIncorrectAdminKey_ThrowsAuthException()
    {
        //Arrange
        var username = "luke";
        var password = "hashedPassword";
        var isAdmin = true;
        var key = "incorrectValue";

        //Act
        Exception exception = await Record.ExceptionAsync(async () =>
        {
            await _identityService.RegisterAsync(username, password, isAdmin, key);
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
        var key = "adminKeyValue";

        _repositoryServiceMock.CreateDbUserAsync(username, Arg.Is<string>(hash => VerifyPassword(password, hash)), isAdmin).Returns(false);

        //Act
        Exception exception = await Record.ExceptionAsync(async () =>
        {
            await _identityService.RegisterAsync(username, password, isAdmin, key);
        });

        //Assert
        exception.Should().BeOfType<Exception>();
        exception.Message.Should().Be("There was an error when creating the user.");
        await _repositoryServiceMock.Received().CreateDbUserAsync(
                    username,
                    Arg.Is<string>(hash => VerifyPassword(password, hash)),
                    isAdmin);
    }

    bool VerifyPassword(string password, string input)
    {
        return BC.Verify(password, input);
    }

    IConfigurationSection GenerateConfigSection()
    {
        var appSettings = @"{""AppSettings"":{
            ""SigningKey"" : ""signingKeyValue"",
            ""AdminKey"" : ""adminKeyValue""
            }}";

        var builder = new ConfigurationBuilder();
        builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));
        var configuration = builder.Build();

        return configuration.GetSection("AppSettings");
    }
}
