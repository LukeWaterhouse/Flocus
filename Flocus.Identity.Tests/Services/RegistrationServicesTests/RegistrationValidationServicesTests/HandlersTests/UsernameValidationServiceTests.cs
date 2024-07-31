using Flocus.Domain.Models.Errors;
using Flocus.Identity.Models;
using Flocus.Identity.Services.RegistrationServices.RegistrationValidationServices.Handlers;
using FluentAssertions;
using Xunit;

namespace Flocus.Identity.Tests.Services.RegistrationServicesTests.RegistrationValidationServicesTests.HandlersTests;

public sealed class UsernameValidationServiceTests
{
    private readonly UsernameValidationHandler _usernameValidationHandler;

    public UsernameValidationServiceTests()
    {
        _usernameValidationHandler = new UsernameValidationHandler();
    }

    [Fact]
    public void Validate_ValidUsername_AddsNoErrors()
    {
        // Arrange
        var username = "lukosparta123";

        var errors = new List<Error>();
        var registrationModel = new RegistrationModel(
            username,
            "password",
            "emailAddress",
            true,
            "123");

        // Act
        _usernameValidationHandler.Validate(errors, registrationModel);

        // Assert
        errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_UsernameTooLong_AddsError()
    {
        // Arrange
        var username = "lukosparta12311111111";

        var errors = new List<Error>();
        var registrationModel = new RegistrationModel(
            username,
            "password",
            "emailAddress",
            true,
            "123");

        // Act
        _usernameValidationHandler.Validate(errors, registrationModel);

        // Assert
        var expectedErrors = new List<Error>()
        {
            new Error(400, $"Username must be less than 20 characters: {username}")
        };

        errors.Should().BeEquivalentTo(expectedErrors);
    }

    [Fact]
    public void Validate_UsernameTooShort_AddsError()
    {
        // Arrange
        var username = "pip";

        var errors = new List<Error>();
        var registrationModel = new RegistrationModel(
            username,
            "password",
            "emailAddress",
            true,
            "123");

        // Act
        _usernameValidationHandler.Validate(errors, registrationModel);

        // Assert
        var expectedErrors = new List<Error>()
        {
            new Error(400, $"Username must be at least 4 characters: {username}")
        };

        errors.Should().BeEquivalentTo(expectedErrors);
    }

    [Fact]
    public void Validate_UsernameContainsWhitespace_AddsError()
    {
        // Arrange
        var username = "luko sparta";

        var errors = new List<Error>();
        var registrationModel = new RegistrationModel(
            username,
            "password",
            "emailAddress",
            true,
            "123");

        // Act
        _usernameValidationHandler.Validate(errors, registrationModel);

        // Assert
        var expectedErrors = new List<Error>()
        {
            new Error(400, $"Username cannot contain whitespace: {username}")
        };

        errors.Should().BeEquivalentTo(expectedErrors);
    }

    [Fact]
    public void Validate_UsernameContainsProfanity_AddsError()
    {
        // Arrange
        var username = "ohmyfuck!";

        var errors = new List<Error>();
        var registrationModel = new RegistrationModel(
            username,
            "password",
            "emailAddress",
            true,
            "123");

        // Act
        _usernameValidationHandler.Validate(errors, registrationModel);

        // Assert
        var expectedErrors = new List<Error>()
        {
            new Error(400, $"Profanity detected in username: {username}")
        };
    }
}
