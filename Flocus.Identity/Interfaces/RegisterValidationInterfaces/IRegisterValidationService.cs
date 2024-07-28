using Flocus.Identity.Models;

namespace Flocus.Identity.Interfaces;

public interface IRegisterValidationService
{
    void ValidateRegistrationModel(RegistrationModel registrationModel);
}
