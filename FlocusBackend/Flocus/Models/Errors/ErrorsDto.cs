namespace Flocus.Models.Errors;

public record struct ErrorsDto(IList<ErrorDto> errors);
