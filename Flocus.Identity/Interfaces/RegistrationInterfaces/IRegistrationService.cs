using Flocus.Identity.Models;

namespace Flocus.Identity.Interfaces.RegisterInterfaces;

public interface IRegistrationService
{
    public Task RegisterAsync(RegistrationModel registrationModel);
}
