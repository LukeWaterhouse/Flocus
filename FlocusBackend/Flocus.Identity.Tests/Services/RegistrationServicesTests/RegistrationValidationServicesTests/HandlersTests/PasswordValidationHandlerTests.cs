using Flocus.Domain.Models.Errors;
using Flocus.Identity.Models;
using Flocus.Identity.Services.RegisterValidation.Handlers;
using FluentAssertions;
using Xunit;

namespace Flocus.Identity.Tests.Services.RegistrationServicesTests.RegistrationValidationServicesTests.HandlersTests;

public sealed class PasswordValidationHandlerTests
{
    private readonly PasswordValidationHandler _passwordValidationHandler;

    private readonly string PasswordRequiredSpecialCharOptions = @"%!@#$%^&*()?/>.<,:;'\|}]{[_~`+=-" + "\"";

    public PasswordValidationHandlerTests()
    {
        _passwordValidationHandler = new PasswordValidationHandler();
    }

    [Fact]
    public void Validate_ValidPassword_AddsNoErrors()
    {
        // Arrange
        var password = "Rollo!234";

        var errors = new List<Error>();
        var registrationModel = new RegistrationModel(
            "username",
            password,
            "emailAddress",
            true,
            "123");

        // Act
        _passwordValidationHandler.Validate(errors, registrationModel);

        // Assert
        errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_PasswordTooLong_AddsError()
    {
        // Arrange
        var password = "Rollo!2345111111111111111111111";

        var errors = new List<Error>();
        var registrationModel = new RegistrationModel(
            "username",
            password,
            "emailAddress",
            true,
            "123");

        // Act
        _passwordValidationHandler.Validate(errors, registrationModel);

        // Assert
        var expectedErrors = new List<Error>()
        {
            new Error(400, $"Password must be less than 30 characters: {password}")
        };

        errors.Should().BeEquivalentTo(expectedErrors);
    }

    [Fact]
    public void Validate_PasswordTooShort_AddsError()
    {
        // Arrange
        var password = "Smol!2";

        var errors = new List<Error>();
        var registrationModel = new RegistrationModel(
            "username",
            password,
            "emailAddress",
            true,
            "123");

        // Act
        _passwordValidationHandler.Validate(errors, registrationModel);

        // Assert
        var expectedErrors = new List<Error>()
        {
            new Error(400, $"Password must be at least 8 characters: {password}")
        };

        errors.Should().BeEquivalentTo(expectedErrors);
    }

    [Fact]
    public void Validate_PasswordContainsWhitespace_AddsError()
    {
        // Arrange
        var password = "Rollo!234 5";

        var errors = new List<Error>();
        var registrationModel = new RegistrationModel(
            "username",
            password,
            "emailAddress",
            true,
            "123");

        // Act
        _passwordValidationHandler.Validate(errors, registrationModel);

        // Assert
        var expectedErrors = new List<Error>()
        {
            new Error(400, $"Password cannot contain whitespace: {password}")
        };

        errors.Should().BeEquivalentTo(expectedErrors);
    }

    [Fact]
    public void Validate_PasswordHasNoLowerCase_AddsError()
    {
        // Arrange
        var password = "ROLLO!2345";

        var errors = new List<Error>();
        var registrationModel = new RegistrationModel(
            "username",
            password,
            "emailAddress",
            true,
            "123");

        // Act
        _passwordValidationHandler.Validate(errors, registrationModel);

        // Assert
        var expectedErrors = new List<Error>()
        {
            new Error(400, $"Password must have at least one upper and lower character: {password}")
        };

        errors.Should().BeEquivalentTo(expectedErrors);
    }

    [Fact]
    public void Validate_PasswordHasNoUpperCase_AddsError()
    {
        // Arrange
        var password = "rollo!2345";

        var errors = new List<Error>();
        var registrationModel = new RegistrationModel(
            "username",
            password,
            "emailAddress",
            true,
            "123");

        // Act
        _passwordValidationHandler.Validate(errors, registrationModel);

        // Assert
        var expectedErrors = new List<Error>()
        {
            new Error(400, $"Password must have at least one upper and lower character: {password}")
        };

        errors.Should().BeEquivalentTo(expectedErrors);
    }

    [Fact]
    public void Validate_PasswordHasNoSpecialCharacters_AddsError()
    {
        // Arrange
        var password = "Rollo12345";

        var errors = new List<Error>();
        var registrationModel = new RegistrationModel(
            "username",
            password,
            "emailAddress",
            true,
            "123");

        // Act
        _passwordValidationHandler.Validate(errors, registrationModel);

        // Assert
        var expectedErrors = new List<Error>()
        {
            new Error(400, $"Password must have at least one special character ({PasswordRequiredSpecialCharOptions}): {password}")
        };

        errors.Should().BeEquivalentTo(expectedErrors);
    }

    [Fact]
    public void Validate_PasswordHasMultipleErrors_AddsAllErrors()
    {
        // Arrange
        var password = "ro llo 111111111111111111111111111111111111";

        var errors = new List<Error>();
        var registrationModel = new RegistrationModel(
            "username",
            password,
            "emailAddress",
            true,
            "123");

        // Act
        _passwordValidationHandler.Validate(errors, registrationModel);

        // Assert
        var expectedErrors = new List<Error>()
        {
            new Error(400, $"Password must be less than 30 characters: {password}"),
            new Error(400, $"Password cannot contain whitespace: {password}"),
            new Error(400, $"Password must have at least one upper and lower character: {password}"),
            new Error(400, $"Password must have at least one special character ({PasswordRequiredSpecialCharOptions}): {password}")
        };

        errors.Should().BeEquivalentTo(expectedErrors);
    }
}
