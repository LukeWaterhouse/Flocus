namespace Flocus.Models.ReturnModels;

public record struct UserDto(
    string EmailAddress,
    DateTime CreatedAt,
    string Username);
