namespace Flocus.Models.Requests;

public record struct RegisterRequestDto(string username, string password, bool isAdmin, string? key);
