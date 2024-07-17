namespace Flocus.Identity.Exceptions;

public sealed class InputValidationException : Exception
{
    public InputValidationException(string message) : base(message) { }
}
