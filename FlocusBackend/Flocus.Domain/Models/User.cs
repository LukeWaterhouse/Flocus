namespace Flocus.Domain.Models;

public sealed record User(
    string ClientId,
    string EmailAddress,
    DateTime CreatedAt,
    string Username,
    bool IsAdmin,
    string PasswordHash);
