namespace Flocus.Domain.Models;

public class User
{
    public string ClientId { get; set; }
    public string EmailAddress { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Username { get; set; }
    public bool IsAdmin { get; set; }
    public string PasswordHash { get; set; }
}
