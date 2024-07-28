using Flocus.Domain.Models.Errors;
using Flocus.Identity.Models;
using System.Net.Mail;

namespace Flocus.Identity.Services.RegisterValidation.Handlers;

public class EmailValidationHandler : BaseRegisterValidationHandler
{
    public EmailValidationHandler(RegistrationModel registrationModel) : base(registrationModel)
    {
    }

    public override List<Error> Validate(List<Error> errors)
    {
        var email = RegistrationModel.EmailAddress;

        try
        {
            var emailAddress = new MailAddress(email);
        }
        catch
        {

            AddBadRequestError(
                $"Email is not a valid format: {email}",
                errors);
        }

        return base.Validate(errors);
    }
}
