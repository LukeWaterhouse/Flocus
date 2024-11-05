namespace Flocus.Repository.Exceptions;

public sealed class DuplicateRecordException : Exception
{
    public DuplicateRecordException(string message) : base(message) { }
}
