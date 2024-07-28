using Flocus.Domain.Models.Errors;
using Flocus.Identity.Exceptions;
using Flocus.Identity.Interfaces;
using Flocus.Identity.Interfaces.RegisterValidationInterfaces;
using Flocus.Identity.Models;

namespace Flocus.Identity.Services.RegisterValidation;

internal class RegistrationValidationService : IRegistrationValidationService
{

    private readonly IRegistrationValidationChainFactory _registrationValidationChainFactory;

    public RegistrationValidationService(IRegistrationValidationChainFactory registrationValidationChainFactory)
    {
        _registrationValidationChainFactory = registrationValidationChainFactory;
    }

    public void InputValidateRegistrationModel(RegistrationModel registrationModel)
    {
        var errors = new List<Error>();

        var registrationValidationChain = _registrationValidationChainFactory.CreateChain();
        registrationValidationChain.Validate(errors, registrationModel);

        if (errors.Any())
        {
            throw new InputValidationException(errors);
        }
    }
}
