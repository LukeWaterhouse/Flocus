namespace Flocus.Models.Requests;

public record struct RegisterRequestDto(string Username, string Password, string EmailAddress, bool IsAdmin, string? Key);
