using Flocus.Domain.Interfaces;
using Flocus.Domain.Models;
using Flocus.Identity.Interfaces.AdminKeyInterfaces;
using Flocus.Identity.Interfaces.PasswordValidationServices;
using Flocus.Identity.Services.RemoveAccountServices;
using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using Xunit;

namespace Flocus.Identity.Tests.Services.RemoveAccountServices;

public sealed class RemoveAccountServiceTests
{
    private readonly IUserRepositoryService _userRepositoryServiceMock;
    private readonly IAdminKeyService _adminKeyServiceMock;
    private readonly IPasswordValidationService _passwordValidationServiceMock;

    private readonly RemoveAccountService _removeAccountService;

    public RemoveAccountServiceTests()
    {
        _userRepositoryServiceMock = Substitute.For<IUserRepositoryService>();
        _adminKeyServiceMock = Substitute.For<IAdminKeyService>();
        _passwordValidationServiceMock = Substitute.For<IPasswordValidationService>();

        _removeAccountService = new RemoveAccountService(
            _userRepositoryServiceMock,
            _adminKeyServiceMock,
            _passwordValidationServiceMock);
    }

    #region DeleteSelfUser
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DeleteSelfUser_ValidUser_DoesNotThrowException(bool isAdmin)
    {
        // Arrange
        var username = "lukosparta123";
        var password = "Password!234";
        var clientId = "clientId123";

        var passwordHash = "123";

        var user = new User(
            clientId,
            "lukerollo@hotmail.com",
            DateTime.UtcNow,
            username,
            isAdmin,
            passwordHash);

        _userRepositoryServiceMock.GetUserAsync(username).Returns(user);

        // Act
        await _removeAccountService.DeleteSelfUserAsync(username, password);

        // Assert
        using (new AssertionScope())
        {
            await _userRepositoryServiceMock.Received(1).GetUserAsync(username);
            await _userRepositoryServiceMock.Received(1).DeleteUser(clientId);
            _passwordValidationServiceMock.Received(1).ValidatePassword(password, passwordHash);
        }
    }
    #endregion

    #region DeleteOtherUser
    [Fact]
    public async Task DeleteOtherUser_NonAdminValidUser_ThrowsNoExceptions()
    {
        // Arrange
        var username = "lukosparta123";
        var clientId = "clientId-123";
        var isAdmin = false;

        var passwordHash = "123";

        var user = new User(
            clientId,
            "lukerollo@hotmail.com",
            DateTime.UtcNow,
            username,
            isAdmin,
            passwordHash);

        _userRepositoryServiceMock.GetUserAsync(username).Returns(user);

        // Act
        await _removeAccountService.DeleteUserByNameAsync(username, null);

        // Assert
        using (new AssertionScope())
        {
            await _userRepositoryServiceMock.Received(1).GetUserAsync(username);
            await _userRepositoryServiceMock.Received(1).DeleteUser(clientId);
            _passwordValidationServiceMock.DidNotReceive().ValidatePassword(Arg.Any<string>(), Arg.Any<string>());
            _adminKeyServiceMock.DidNotReceive().CheckAdminKeyCorrect(Arg.Any<string>());
        }
    }

    [Fact]
    public async Task DeleteOtherUser_AdminValidUserWithKey_ThrowsNoExceptions()
    {
        // Arrange
        var username = "lukosparta123";
        var isAdmin = true;
        var passwordHash = "123";
        var adminKey = "1234";
        var clientId = "12345";

        var user = new User(
            clientId,
            "lukerollo@hotmail.com",
            DateTime.UtcNow,
            username,
            isAdmin,
            passwordHash);

        _userRepositoryServiceMock.GetUserAsync(username).Returns(user);

        // Act
        await _removeAccountService.DeleteUserByNameAsync(username, adminKey);

        // Assert
        using (new AssertionScope())
        {
            await _userRepositoryServiceMock.Received(1).GetUserAsync(username);
            await _userRepositoryServiceMock.Received(1).DeleteUser(clientId);
            _adminKeyServiceMock.Received(1).CheckAdminKeyCorrect(adminKey);
            _passwordValidationServiceMock.DidNotReceive().ValidatePassword(Arg.Any<string>(), Arg.Any<string>());
        }
    }

    [Fact]
    public async Task DeleteOtherUser_AdminValidUserWithNoKey_ThrowsException()
    {
        // Arrange
        var username = "lukosparta123";
        var isAdmin = true;
        var passwordHash = "123";

        var user = new User(
            "clientId123",
            "lukerollo@hotmail.com",
            DateTime.UtcNow,
            username,
            isAdmin,
            passwordHash);

        _userRepositoryServiceMock.GetUserAsync(username).Returns(user);

        // Act
        Exception exception = await Record.ExceptionAsync(async () =>
        {
            await _removeAccountService.DeleteUserByNameAsync(username, null);
        });

        // Assert
        using (new AssertionScope())
        {
            exception.Should().BeOfType<UnauthorizedAccessException>();
            exception.Message.Should().Be($"You must provide an admin key when deleting a different admin account: {username}");
            await _userRepositoryServiceMock.Received(1).GetUserAsync(username);
            _passwordValidationServiceMock.DidNotReceive().ValidatePassword(Arg.Any<string>(), Arg.Any<string>());
            _passwordValidationServiceMock.DidNotReceive().ValidatePassword(Arg.Any<string>(), Arg.Any<string>());
            _adminKeyServiceMock.DidNotReceive().CheckAdminKeyCorrect(Arg.Any<string>());
            await _userRepositoryServiceMock.DidNotReceive().DeleteUser(Arg.Any<string>());

        }
    }
    #endregion
}
