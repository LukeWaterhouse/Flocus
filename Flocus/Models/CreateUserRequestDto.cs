namespace Flocus.Models;

public record struct CreateUserRequestDto(string username, string password, bool isAdmin, string? key);
