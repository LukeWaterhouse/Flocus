using Flocus.Identity.Interfaces.RegisterValidationInterfaces;
using Flocus.Identity.Services.RegisterValidation.Handlers;
using Flocus.Identity.Services.RegistrationServices.RegistrationValidationServices.Factories;
using Flocus.Identity.Services.RegistrationServices.RegistrationValidationServices.Handlers;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Flocus.Identity.Tests.Services.RegistrationServicesTests.RegistrationValidationServicesTests.FactoriesTests;

public sealed class RegistrationValidationChainFactoryTests
{
    private readonly EmailValidationHandler _emailValidationHandlerMock;
    private readonly IsAdminValidationHandler _isAdminValidationHandlerMock;
    private readonly PasswordValidationHandler _passwordValidationHandlerMock;
    private readonly UsernameValidationHandler _usernameValidationHandlerMock;

    private readonly RegistrationValidationChainFactory _registrationValidationChainFactory;

    public RegistrationValidationChainFactoryTests()
    {
        _emailValidationHandlerMock = new EmailValidationHandler();
        _isAdminValidationHandlerMock = new IsAdminValidationHandler();
        _passwordValidationHandlerMock = new PasswordValidationHandler();
        _usernameValidationHandlerMock = new UsernameValidationHandler();

        _registrationValidationChainFactory = new RegistrationValidationChainFactory(
            _emailValidationHandlerMock,
            _isAdminValidationHandlerMock,
            _passwordValidationHandlerMock,
            _usernameValidationHandlerMock);
    }

    [Fact]
    public void CreateChain_ValidHandlers_CreatesChain()
    {
        // Act
        var validator = _registrationValidationChainFactory.CreateChain();

        // Assert
        using (new AssertionScope())
        {
            var usernameValidator = AssertValidatorType<UsernameValidationHandler>(validator);
            var passwordValidator = AssertValidatorType<PasswordValidationHandler>(usernameValidator._nextHandler!);
            var emailValidator = AssertValidatorType<EmailValidationHandler>(passwordValidator._nextHandler!);
            var isAdminValidator = AssertValidatorType<IsAdminValidationHandler>(emailValidator._nextHandler!);
        }
    }

    public static T AssertValidatorType<T>(IRegistrationValidationHandler validator) where T : class, IRegistrationValidationHandler
    {
        validator.Should().BeOfType<T>();

        var castedValidator = validator as T;
        return castedValidator!;
    }
}
