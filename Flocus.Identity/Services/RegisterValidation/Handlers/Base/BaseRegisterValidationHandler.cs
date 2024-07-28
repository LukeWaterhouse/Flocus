using Flocus.Domain.Models.Errors;
using Flocus.Identity.Interfaces.RegisterValidationInterfaces;
using Flocus.Identity.Models;
using Microsoft.AspNetCore.Http;

namespace Flocus.Identity.Services.RegisterValidation.Handlers;

public class BaseRegisterValidationHandler : IRegisterValidationHandler
{

    public IRegisterValidationHandler? _nextHandler;

    public RegistrationModel RegistrationModel;

    internal readonly string CharacterMinimumError = "{0} must be at least {1} characters: {2}";
    internal readonly string CharacterLimitError = "{0} must be less than {1} characters: {2}";
    internal readonly string ContainsWhitespaceError = "{0} cannot contain whitespace: {1}";

    public BaseRegisterValidationHandler(RegistrationModel registrationModel)
    {
        RegistrationModel = registrationModel;
    }

    public IRegisterValidationHandler SetNextValidator(IRegisterValidationHandler handler)
    {
        _nextHandler = handler;
        return handler;
    }

    public virtual List<Error> Validate(List<Error> errors)
    {
        if (_nextHandler != null)
        {
            return _nextHandler.Validate(errors);
        }
        else
        {
            return errors;
        }
    }

    internal void AddBadRequestError(string errorMessage, List<Error> errors) {
        var error = new Error(StatusCodes.Status400BadRequest, errorMessage);
        errors.Add(error);
    }
}