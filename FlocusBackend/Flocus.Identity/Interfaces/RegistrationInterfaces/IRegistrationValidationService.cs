using Flocus.Identity.Models;

namespace Flocus.Identity.Interfaces;

public interface IRegistrationValidationService
{
    void InputValidateRegistrationModel(RegistrationModel registrationModel);
}
