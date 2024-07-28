using Flocus.Identity.Interfaces.RegisterValidationInterfaces;
using Flocus.Identity.Models;
using Flocus.Identity.Services.RegisterValidation.Handlers;

namespace Flocus.Identity.Services.RegisterValidation.Factories;

public class RegistrationValidationChainFactory : IRegistrationValidationChainFactory
{
    public IRegisterValidationHandler CreateChain(RegistrationModel registrationModel)
    {
        var usernameValidationHandler = new UsernameValidationHandler(registrationModel);
        var passwordValidationHandler = new PasswordValidationHandler(registrationModel);
        var emailValidationHandler = new EmailValidationHandler(registrationModel);
        var isAdminValidationHandler = new IsAdminValidationHandler(registrationModel);

        usernameValidationHandler
            .SetNextValidator(passwordValidationHandler)
            .SetNextValidator(emailValidationHandler)
            .SetNextValidator(isAdminValidationHandler);


        return usernameValidationHandler;
    }
}
