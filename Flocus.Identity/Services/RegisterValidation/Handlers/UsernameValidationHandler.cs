using Flocus.Domain.Models.Errors;
using Flocus.Identity.Models;

namespace Flocus.Identity.Services.RegisterValidation.Handlers;

public class UsernameValidationHandler : BaseRegisterValidationHandler
{
    private readonly int UsernameLengthLimit = 20;
    private readonly int UsernameLengthMinimum = 4;

    private ProfanityFilter.ProfanityFilter ProfanityFilter = new ProfanityFilter.ProfanityFilter();

    public UsernameValidationHandler(RegistrationModel registrationModel) : base(registrationModel)
    {
    }

    public override List<Error> Validate(List<Error> errors)
    {
        var usernameReference = "Username";
        var username = RegistrationModel.Username;

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

        return base.Validate(errors);
    }
}
