using Flocus.Domain.Models.Errors;
using Flocus.Identity.Models;
using Flocus.Identity.Services.RegistrationServices.RegistrationValidationServices.Handlers.Base;

namespace Flocus.Identity.Services.RegistrationServices.RegistrationValidationServices.Handlers;

public sealed class UsernameValidationHandler : BaseRegistrationValidationHandler
{
    private readonly int UsernameLengthLimit = 20;
    private readonly int UsernameLengthMinimum = 4;

    private ProfanityFilter.ProfanityFilter ProfanityFilter = new ProfanityFilter.ProfanityFilter();

    public override (List<Error>, RegistrationModel) Validate(List<Error> errors, RegistrationModel registrationModel)
    {
        var usernameReference = "Username";
        var username = registrationModel.Username;

        if (username.Length > UsernameLengthLimit)
        {
            AddBadRequestError(
                string.Format(CharacterLimitError, usernameReference, UsernameLengthLimit, username),
                errors);
        }

        if (username.Length < UsernameLengthMinimum)
        {
            AddBadRequestError(
                string.Format(CharacterMinimumError, usernameReference, UsernameLengthMinimum, username),
                errors);
        }

        if (username.Contains(" "))
        {
            AddBadRequestError(
                string.Format(ContainsWhitespaceError, usernameReference, username),
                errors);
        }

        if (ProfanityFilter.ContainsProfanity(username))
        {
            AddBadRequestError(
                $"Profanity detected in username: {username}",
                errors);
        }

        return base.Validate(errors, registrationModel);
    }
}
