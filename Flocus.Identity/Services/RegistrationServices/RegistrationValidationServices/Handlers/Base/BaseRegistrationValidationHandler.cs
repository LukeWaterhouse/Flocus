using Flocus.Domain.Models.Errors;
using Flocus.Identity.Interfaces.RegisterValidationInterfaces;
using Flocus.Identity.Models;
using Microsoft.AspNetCore.Http;

namespace Flocus.Identity.Services.RegistrationServices.RegistrationValidationServices.Handlers.Base;

public abstract class BaseRegistrationValidationHandler : IRegistrationValidationHandler
{

    public IRegistrationValidationHandler? _nextHandler;

    internal readonly string CharacterMinimumError = "{0} must be at least {1} characters: {2}";
    internal readonly string CharacterLimitError = "{0} must be less than {1} characters: {2}";
    internal readonly string ContainsWhitespaceError = "{0} cannot contain whitespace: {1}";

    public IRegistrationValidationHandler SetNextValidator(IRegistrationValidationHandler handler)
    {
        _nextHandler = handler;
        return handler;
    }

    public virtual (List<Error>, RegistrationModel) Validate(List<Error> errors, RegistrationModel registrationModel)
    {
        if (_nextHandler != null)
        {
            return _nextHandler.Validate(errors, registrationModel);
        }
        else
        {
            return (errors, registrationModel);
        }
    }

    internal void AddBadRequestError(string errorMessage, List<Error> errors)
    {
        var error = new Error(StatusCodes.Status400BadRequest, errorMessage);
        errors.Add(error);
    }
}
