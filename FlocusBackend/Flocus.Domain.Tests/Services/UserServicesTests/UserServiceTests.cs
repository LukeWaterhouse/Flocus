using Flocus.Domain.Interfaces;
using Flocus.Domain.Models;
using Flocus.Domain.Services.UserServices;
using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using Xunit;

namespace Flocus.Domain.Tests.Services.UserServicesTests;

public sealed class UserServiceTests
{
    private readonly IUserRepositoryService _userRepositoryServiceMock;
    private readonly IUserService _userService;

    public UserServiceTests()
    {
        _userRepositoryServiceMock = Substitute.For<IUserRepositoryService>();
        _userService = new UserService(_userRepositoryServiceMock);
    }

    [Fact]
    public async Task GetUserAsync_ValidUsername_ReturnsUser()
    {
        // Arrange
        var username = "lukosparta123";
        var clientId = Guid.NewGuid().ToString();
        var email = "lukosparta@hotmail.com";
        var createdAt = DateTime.UtcNow;
        var isAdmin = true;
        var passwordHash = "19lk234l534l5tk";

        var user = new User(clientId, email, createdAt, username, isAdmin, passwordHash);
        _userRepositoryServiceMock.GetUserAsync(username).Returns(user);

        // Act
        var result = await _userService.GetUserAsync(username);

        // Assert
        var expectedUser = new User("asd", email, createdAt, username, isAdmin, passwordHash);

        using (new AssertionScope())
        {
            result.Should().BeEquivalentTo(expectedUser);
        }
    }
}
