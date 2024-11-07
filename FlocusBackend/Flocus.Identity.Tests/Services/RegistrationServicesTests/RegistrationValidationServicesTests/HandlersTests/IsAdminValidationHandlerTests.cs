using Flocus.Domain.Models.Errors;
using Flocus.Identity.Models;
using Flocus.Identity.Services.RegistrationServices.RegistrationValidationServices.Handlers;
using FluentAssertions;
using Xunit;

namespace Flocus.Identity.Tests.Services.RegistrationServicesTests.RegistrationValidationServicesTests.HandlersTests;

public sealed class IsAdminValidationHandlerTests
{
    private readonly IsAdminValidationHandler _isAdminValidationHandler;

    public IsAdminValidationHandlerTests()
    {
        _isAdminValidationHandler = new IsAdminValidationHandler();
    }

    [Fact]
    public void Validate_IsAdminTrueAndKey_AddsNoErrors()
    {
        // Arrange
        var isAdmin = true;
        var adminKey = "123";

        var errors = new List<Error>();
        var registrationModel = new RegistrationModel(
            "username",
            "password",
            "emailAddress",
            isAdmin,
            adminKey);

        // Act
        _isAdminValidationHandler.Validate(errors, registrationModel);

        // Assert
        errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_IsAdminFalseAndNoKey_AddsNoErrors()
    {
        // Arrange
        var isAdmin = false;
        string? adminKey = null;

        var errors = new List<Error>();
        var registrationModel = new RegistrationModel(
            "username",
            "password",
            "emailAddress",
            isAdmin,
            adminKey);

        // Act
        _isAdminValidationHandler.Validate(errors, registrationModel);

        // Assert
        errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_IsAdminTrueAndNoKey_AddsError()
    {
        // Arrange
        var isAdmin = true;
        string? adminKey = null;

        var errors = new List<Error>();
        var registrationModel = new RegistrationModel(
            "username",
            "password",
            "emailAddress",
            isAdmin,
            adminKey);

        // Act
        _isAdminValidationHandler.Validate(errors, registrationModel);

        // Assert
        var expectedErrors = new List<Error>()
        {
            new Error(400,  "Must provide key when creating an admin")
        };

        errors.Should().BeEquivalentTo(expectedErrors);
    }
}
