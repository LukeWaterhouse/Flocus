namespace Flocus.Models.Requests;

public record struct RegisterRequestDto(string username, string password, string emailAddress, bool isAdmin, string? key);
