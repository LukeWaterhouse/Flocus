using Flocus.Domain.Models.Errors;
using Flocus.Identity.Exceptions;
using Flocus.Identity.Interfaces;
using Flocus.Identity.Interfaces.RegisterValidationInterfaces;
using Flocus.Identity.Models;
using Flocus.Identity.Services.RegisterValidation;
using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using Xunit;

namespace Flocus.Identity.Tests.Services.RegistrationServicesTests.RegistrationValidationServicesTests;

public sealed class RegistrationValidationServiceTests
{
    private readonly IRegistrationValidationChainFactory _registrationValidationChainFactoryMock;
    private readonly IRegistrationValidationService _registrationValidationService;

    public RegistrationValidationServiceTests()
    {
        _registrationValidationChainFactoryMock = Substitute.For<IRegistrationValidationChainFactory>();
        _registrationValidationService = new RegistrationValidationService(_registrationValidationChainFactoryMock);
    }

    [Fact]
    public void InputValidateRegistrationModel_ValidRegistrationModel_DoesNotThrowException()
    {
        // Arrange
        var registrationModel = new RegistrationModel(
            "username",
            "password",
            "email@email.com",
            true,
            "123");

        var registrationHandlerMock = Substitute.For<IRegistrationValidationHandler>();
        _registrationValidationChainFactoryMock.CreateChain().Returns(registrationHandlerMock);

        // Act
        _registrationValidationService.InputValidateRegistrationModel(registrationModel);

        // Assert
        using (new AssertionScope())
        {
            registrationHandlerMock.Received(1).Validate(Arg.Any<List<Error>>(), registrationModel);
        }
    }

    [Fact]
    public void InputValidateRegistrationModel_InvalidRegistrationModel_ThrowsException()
    {
        // Arrange
        var registrationModel = new RegistrationModel(
            "username",
            "password",
            "email@email.com",
            true,
            "123");

        var registrationHandlerMock = Substitute.For<IRegistrationValidationHandler>();
        registrationHandlerMock
            .When(x => x.Validate(Arg.Any<List<Error>>(), registrationModel))
            .Do(callInfo =>
            {
                var errorList = callInfo.Arg<List<Error>>();
                errorList.Add(new Error(400, "error message"));
            });

        _registrationValidationChainFactoryMock.CreateChain().Returns(registrationHandlerMock);

        //Act
        Exception exception = Record.Exception(() =>
        {
            _registrationValidationService.InputValidateRegistrationModel(registrationModel);
        });

        // Assert
        var expectedErrors = new List<Error>()
        {
            new Error(400, "error message")
        };

        using (new AssertionScope())
        {
            exception.Should().BeOfType<InputValidationException>();
            var inputValidationException = exception as InputValidationException;
            inputValidationException!.Errors.Should().BeEquivalentTo(expectedErrors);
        }
    }
}
