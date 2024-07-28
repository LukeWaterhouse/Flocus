using Flocus.Domain.Models.Errors;
using Flocus.Identity.Models;
using Flocus.Identity.Services.RegisterValidation.Handlers;
using System.Net.Mail;

namespace Flocus.Identity.Services.RegistrationServices.RegistrationValidationServices.Handlers;

public sealed class EmailValidationHandler : BaseRegistrationValidationHandler
{

    public override (List<Error>, RegistrationModel) Validate(List<Error> errors, RegistrationModel registrationModel)
    {
        var email = registrationModel.EmailAddress;

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

        return base.Validate(errors, registrationModel);
    }
}
