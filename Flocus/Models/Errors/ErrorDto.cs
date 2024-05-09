namespace Flocus.Models.Errors;

public readonly record struct ErrorDto(int status, string message);
