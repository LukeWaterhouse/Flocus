using Flocus.Domain.Models.Errors;
using Flocus.Identity.Models;

namespace Flocus.Identity.Services.RegisterValidation.Handlers;

public class IsAdminValidationHandler : BaseRegisterValidationHandler
{
    public IsAdminValidationHandler(RegistrationModel registrationModel) : base(registrationModel)
    {
    }

    public override List<Error> Validate(List<Error> errors)
    {
        if (RegistrationModel.IsAdmin && RegistrationModel.Key == null)
        {
            AddBadRequestError(
                "Must provide key when creating an admin",
                errors);
        }

        return base.Validate(errors);
    }
}
