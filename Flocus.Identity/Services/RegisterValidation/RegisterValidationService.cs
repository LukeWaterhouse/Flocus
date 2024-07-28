using Flocus.Domain.Models.Errors;
using Flocus.Identity.Exceptions;
using Flocus.Identity.Interfaces;
using Flocus.Identity.Interfaces.RegisterValidationInterfaces;
using Flocus.Identity.Models;

namespace Flocus.Identity.Services.RegisterValidation;

internal class RegisterValidationService : IRegisterValidationService
{

    private readonly IRegistrationValidationChainFactory _registrationValidationChainFactory;

    public RegisterValidationService(IRegistrationValidationChainFactory registrationValidationChainFactory)
    {
        _registrationValidationChainFactory = registrationValidationChainFactory;
    }

    public void ValidateRegistrationModel(RegistrationModel registrationModel)
    {
        var errors = new List<Error>();

        var registrationValidationChain = _registrationValidationChainFactory.CreateChain(registrationModel);
        registrationValidationChain.Validate(errors);

        if (errors.Any())
        {
            throw new InputValidationException(errors);
        }
    }
}
