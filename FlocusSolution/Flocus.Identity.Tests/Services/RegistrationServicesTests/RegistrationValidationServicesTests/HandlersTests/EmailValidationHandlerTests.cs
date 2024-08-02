using Flocus.Domain.Models.Errors;
using Flocus.Identity.Models;
using Flocus.Identity.Services.RegistrationServices.RegistrationValidationServices.Handlers;
using FluentAssertions;
using Xunit;

namespace Flocus.Identity.Tests.Services.RegistrationServicesTests.RegistrationValidationServicesTests.HandlersTests;

public sealed class EmailValidationHandlerTests
{
    private readonly EmailValidationHandler _emailValidationHandler;

    public EmailValidationHandlerTests()
    {
        _emailValidationHandler = new EmailValidationHandler();
    }

    [Fact]
    public void Validate_ValidEmail_AddsNoErrors()
    {
        // Arrange
        var email = "luke@hotmail.co.uk";

        var errors = new List<Error>();
        var registrationModel = new RegistrationModel(
            "username",
            "password",
            email,
            true,
            "123");

        // Act
        _emailValidationHandler.Validate(errors, registrationModel);

        // Assert
        errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_InvalidEmail_AddsError()
    {
        // Arrange
        var email = "invalid email";

        var errors = new List<Error>();
        var registrationModel = new RegistrationModel(
            "username",
            "password",
            email,
            true,
            "123");

        // Act
        _emailValidationHandler.Validate(errors, registrationModel);

        // Assert
        var expectedErrors = new List<Error>()
        {
            new Error(400, $"Email is not a valid format: {email}")

        };

        errors.Should().BeEquivalentTo(expectedErrors);
    }
}
