using Flocus.Domain.Models.Errors;
using Flocus.Identity.Models;
using Flocus.Identity.Services.RegistrationServices.RegistrationValidationServices.Handlers.Base;

namespace Flocus.Identity.Services.RegistrationServices.RegistrationValidationServices.Handlers;

public sealed class IsAdminValidationHandler : BaseRegistrationValidationHandler
{
    public override (List<Error>, RegistrationModel) Validate(List<Error> errors, RegistrationModel registrationModel)
    {
        var adminKey = registrationModel.Key;
        var isAdmin = registrationModel.IsAdmin;

        if (isAdmin && adminKey == null)
        {
            AddBadRequestError(
                "Must provide key when creating an admin",
                errors);
        }

        return base.Validate(errors, registrationModel);
    }
}
