namespace Flocus.Domain.Models.Errors;

public record struct Error(int Status, string Message);
