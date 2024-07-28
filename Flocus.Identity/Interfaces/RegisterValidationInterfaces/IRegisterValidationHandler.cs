using Flocus.Domain.Models.Errors;
using Flocus.Identity.Models;

namespace Flocus.Identity.Interfaces.RegisterValidationInterfaces;

public interface IRegisterValidationHandler
{
    IRegisterValidationHandler SetNextValidator(IRegisterValidationHandler handler);

    List<Error> Validate(List<Error> errors);
}
