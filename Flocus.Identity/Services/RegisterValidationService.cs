using Flocus.Identity.Exceptions;
using Flocus.Identity.Interfaces;
using Flocus.Identity.Models;
using System.Net.Mail;

namespace Flocus.Identity.Services;

internal class RegisterValidationService : IRegisterValidationService
{
    private readonly int UsernameLengthLimit = 20;
    private readonly int PasswordLengthLimit = 30;

    public void ValidateRegistrationModel(RegistrationModel registrationModel)
    {

        if (registrationModel.Username.Length > UsernameLengthLimit)
        {
            throw new InputValidationException($"Username must be less than {UsernameLengthLimit} characters: {registrationModel.Username}");
        }

        if (registrationModel.Password.Length > PasswordLengthLimit)
        {
            throw new InputValidationException($"Password must be less than {PasswordLengthLimit} characters: {registrationModel.Password}");
        }

        if (!IsValid(registrationModel.EmailAddress))
        {
            throw new InputValidationException($"Email is not a valid format : {registrationModel.EmailAddress}");
        }

        if (registrationModel.IsAdmin && registrationModel.Key == null)
        {
            throw new InputValidationException("Must provide key when creating an admin");
        }
    }

    private static bool IsValid(string email)
    {
        var valid = true;

        try
        {
            var emailAddress = new MailAddress(email);
        }
        catch
        {
            valid = false;
        }

        return valid;
    }
}
