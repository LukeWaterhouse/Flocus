namespace Flocus.Identity.Models;

public sealed record RegistrationModel(string Username, string Password, string EmailAddress, bool IsAdmin, string? Key);