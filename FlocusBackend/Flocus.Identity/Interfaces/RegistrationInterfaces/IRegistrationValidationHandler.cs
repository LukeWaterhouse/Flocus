using Flocus.Domain.Models.Errors;
using Flocus.Identity.Models;

namespace Flocus.Identity.Interfaces.RegisterValidationInterfaces;

public interface IRegistrationValidationHandler
{
    IRegistrationValidationHandler SetNextValidator(IRegistrationValidationHandler handler);

    (List<Error>, RegistrationModel) Validate(List<Error> errors, RegistrationModel registrationModel);
}
