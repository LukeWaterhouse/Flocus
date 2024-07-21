using Flocus.Identity.Exceptions;
using Flocus.Identity.Interfaces;
using Flocus.Identity.Models;
using System.Net.Mail;

namespace Flocus.Identity.Services;

internal class RegisterValidationService : IRegisterValidationService
{
    private ProfanityFilter.ProfanityFilter ProfanityFilter = new ProfanityFilter.ProfanityFilter();

    private readonly int UsernameLengthLimit = 20;
    private readonly int UsernameLengthMinimum = 4;

    private readonly int PasswordLengthLimit = 30;
    private readonly int PasswordLengthMinimum = 8;
    private readonly string PasswordRequiredSpecialCharOptions = @"%!@#$%^&*()?/>.<,:;'\|}]{[_~`+=-" + "\"";

    private readonly string CharacterMinimumError = "{0} must be at least {1} characters: {2}";
    private readonly string CharacterLimitError = "{0} must be less than {1} characters: {2}";

    private readonly string ContainsWhitespaceError = "{0} cannot contain whitespace: {1}";

    public RegisterValidationService()
    {
    }

    public void ValidateRegistrationModel(RegistrationModel registrationModel)
    {
        if (registrationModel.IsAdmin && registrationModel.Key == null)
        {
            throw new InputValidationException("Must provide key when creating an admin");
        }

        ValidateUsername(registrationModel.Username);
        ValidateEmailAddress(registrationModel.EmailAddress);
        ValidatePassword(registrationModel.Password);
    }

    private void ValidateUsername(string username)
    {
        var usernameReference = "Username";

        if (username.Length > UsernameLengthLimit)
        {
            throw new InputValidationException(string.Format(CharacterLimitError, usernameReference, UsernameLengthLimit, username));
        }

        if (username.Length < UsernameLengthMinimum)
        {
            throw new InputValidationException(string.Format(CharacterMinimumError, usernameReference, UsernameLengthMinimum, username));
        }

        if (username.Contains(" "))
        {
            throw new InputValidationException(string.Format(ContainsWhitespaceError, usernameReference, username));
        }

        if (ProfanityFilter.ContainsProfanity(username))
        {
            throw new InputValidationException($"Profanity detected in username: {username}");
        }
    }

    private void ValidatePassword(string password)
    {
        var passwordReference = "Password";

        if (password.Length > PasswordLengthLimit)
        {
            throw new InputValidationException(string.Format(CharacterLimitError, passwordReference, PasswordLengthLimit, password));
        }

        if (password.Length < PasswordLengthMinimum)
        {
            throw new InputValidationException(string.Format(CharacterMinimumError, passwordReference, PasswordLengthMinimum, password));
        }

        if (password.Contains(" "))
        {
            throw new InputValidationException(string.Format(ContainsWhitespaceError, passwordReference, password));
        }

        if (!password.Any(char.IsLower) || !password.Any(char.IsUpper))
        {
            throw new InputValidationException($"{passwordReference} must have at least one upper and lower character: {password}");
        }

        if (!ContainsSpecialChar(password))
        {
            throw new InputValidationException($"{passwordReference} must have at least one special character ({PasswordRequiredSpecialCharOptions}): {password}");
        }
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

    private void ValidateEmailAddress(string email)
    {
        try
        {
            var emailAddress = new MailAddress(email);
        }
        catch
        {
            throw new InputValidationException($"Email is not a valid format: {email}");
        }
    }
}
