using Flocus.Identity.Models;

namespace Flocus.Identity.Interfaces.RegisterValidationInterfaces;

public interface IRegistrationValidationChainFactory
{
    IRegistrationValidationHandler CreateChain();
}
