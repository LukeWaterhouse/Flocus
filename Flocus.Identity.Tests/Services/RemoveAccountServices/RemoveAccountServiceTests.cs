using Flocus.Domain.Interfaces;
using Flocus.Domain.Models;
using Flocus.Identity.Interfaces.AdminKeyInterfaces;
using Flocus.Identity.Interfaces.PasswordValidationServices;
using Flocus.Identity.Services.RemoveAccountServices;
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

    [Fact]
    public async Task DeleteUserAsUser_ValidUser_DoesNotThrowException()
    {
        // Arrange
        var username = "lukosparta123";
        var password = "Password!234";
        var isAdmin = false;

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
        await _removeAccountService.DeleteUserAsUserAsync(username, password);

        // Assert
        using (new AssertionScope())
        {
            await _userRepositoryServiceMock.Received(1).GetUserAsync(username);
        }
    }
}
