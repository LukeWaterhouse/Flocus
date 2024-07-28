using Flocus.Domain.Models.Errors;

namespace Flocus.Identity.Exceptions;

public sealed class InputValidationException : Exception
{
    public List<Error> Errors { get; init; }

    public InputValidationException(List<Error> errors) : base("Input validation error(s).") 
    { 
        Errors = errors;
    }
}
