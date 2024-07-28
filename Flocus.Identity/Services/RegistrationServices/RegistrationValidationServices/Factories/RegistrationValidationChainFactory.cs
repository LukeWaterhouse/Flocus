using Flocus.Identity.Interfaces.RegisterValidationInterfaces;
using Flocus.Identity.Services.RegisterValidation.Handlers;
using Flocus.Identity.Services.RegistrationServices.RegistrationValidationServices.Handlers;

namespace Flocus.Identity.Services.RegistrationServices.RegistrationValidationServices.Factories;

public class RegistrationValidationChainFactory : IRegistrationValidationChainFactory
{
    private readonly EmailValidationHandler _emailValidationHandler;
    private readonly IsAdminValidationHandler _isAdminValidationHandler;
    private readonly PasswordValidationHandler _passwordValidationHandler;
    private readonly UsernameValidationHandler _usernameValidationHandler;

    public RegistrationValidationChainFactory(
        EmailValidationHandler emailValidationHandler,
        IsAdminValidationHandler isAdminValidationHandler,
        PasswordValidationHandler passwordValidationHandler,
        UsernameValidationHandler usernameValidationHandler)
    {
        _emailValidationHandler = emailValidationHandler;
        _passwordValidationHandler = passwordValidationHandler;
        _usernameValidationHandler = usernameValidationHandler;
        _isAdminValidationHandler = isAdminValidationHandler;
    }

    public IRegistrationValidationHandler CreateChain()
    {
        _usernameValidationHandler
            .SetNextValidator(_passwordValidationHandler)
            .SetNextValidator(_emailValidationHandler)
            .SetNextValidator(_isAdminValidationHandler);

        return _usernameValidationHandler;
    }
}
