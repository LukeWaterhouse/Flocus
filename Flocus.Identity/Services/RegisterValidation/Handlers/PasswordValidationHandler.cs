using Flocus.Domain.Models.Errors;
using Flocus.Identity.Models;

namespace Flocus.Identity.Services.RegisterValidation.Handlers;

public class PasswordValidationHandler : BaseRegisterValidationHandler
{
    private readonly int PasswordLengthLimit = 30;
    private readonly int PasswordLengthMinimum = 8;
    private readonly string PasswordRequiredSpecialCharOptions = @"%!@#$%^&*()?/>.<,:;'\|}]{[_~`+=-" + "\"";

    public PasswordValidationHandler(RegistrationModel registrationModel) : base(registrationModel)
    {
    }

    public override List<Error> Validate(List<Error> errors)
    {
        var passwordReference = "Password";
        var password = RegistrationModel.Password;

        if (password.Length > PasswordLengthLimit)
        {
            AddBadRequestError(
                string.Format(CharacterLimitError, passwordReference, PasswordLengthLimit, password),
                errors);
        }

        if (password.Length < PasswordLengthMinimum)
        {
            AddBadRequestError(
                string.Format(CharacterMinimumError, passwordReference, PasswordLengthMinimum, password),
                errors);
        }

        if (password.Contains(" "))
        {
            AddBadRequestError(
                string.Format(ContainsWhitespaceError, passwordReference, password),
                errors);
        }

        if (!password.Any(char.IsLower) || !password.Any(char.IsUpper))
        {
            AddBadRequestError(
                $"{passwordReference} must have at least one upper and lower character: {password}",
                errors);
        }

        if (!ContainsSpecialChar(password))
        {
            AddBadRequestError(
                $"{passwordReference} must have at least one special character ({PasswordRequiredSpecialCharOptions}): {password}",
                errors);
        }

        return base.Validate(errors);
    }

    private bool ContainsSpecialChar(string password)
    {
        var passwordContainsRequiredSpecialChar = false;
        foreach (var ch in PasswordRequiredSpecialCharOptions)
        {
            if (password.Contains(ch))
                passwordContainsRequiredSpecialChar = true;
        }
        return passwordContainsRequiredSpecialChar;
    }
}
