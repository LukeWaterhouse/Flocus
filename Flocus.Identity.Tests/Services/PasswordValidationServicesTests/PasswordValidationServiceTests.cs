using Flocus.Identity.Services.PasswordValidationServices;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Security.Authentication;
using Xunit;

namespace Flocus.Identity.Tests.Services.PasswordValidationServicesTests;

public sealed class PasswordValidationServiceTests
{
    private readonly PasswordValidationService _passwordValidationService;

    public PasswordValidationServiceTests()
    {
        _passwordValidationService = new PasswordValidationService();
    }

    [Fact]
    public void ValidatePassword_CorrectPassword_ThrowsNoExceptions()
    {
        // Arrange
        var correctPassword = "Password!234";
        var hashedPassword = "$2a$11$ERNyOOsA9XNX6Ff2QF3ZyOp3Ap7tptk4aeIDjlcNOeeCLqD/XUVg6";

        // Act
        _passwordValidationService.ValidatePassword(correctPassword, hashedPassword);
    }

    [Fact]
    public void ValidatePassword_IncorrectPassword_ThrowsException()
    {
        // Arrange
        var incorrectPassword = "IncorrectPassword!234";
        var hashedPassword = "$2a$11$ERNyOOsA9XNX6Ff2QF3ZyOp3Ap7tptk4aeIDjlcNOeeCLqD/XUVg6";

        // Act
        Exception exception = Record.Exception(() =>
        {
            _passwordValidationService.ValidatePassword(incorrectPassword, hashedPassword);
        });

        // Assert
        using (new AssertionScope())
        {
            exception.Should().BeOfType<AuthenticationException>();
            exception.Message.Should().Be("Invalid username and password combination");
        }
    }

    [Fact]
    public void GetIncorrectPasswordMessage_ValidCall_GetsCorrectMessage()
    {
        // Act
        var result = _passwordValidationService.IncorrectUsernamePasswordMessage;

        // Assert
        result.Should().Be("Invalid username and password combination");
    }
}
