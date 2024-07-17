namespace Flocus.Identity.Models;

public sealed class Claims
{
    public string Username { get; }
    public string EmailAddress { get; }
    public string Role { get; }
    public DateTime Expiry { get; }

    public Claims(string Username, string EmailAddress, string Role, DateTime? Expiry)
    {
        this.Username = Username ?? throw new ArgumentNullException(nameof(Username));
        this.EmailAddress = EmailAddress ?? throw new ArgumentNullException(nameof(EmailAddress));
        this.Role = Role ?? throw new ArgumentNullException(nameof(Role));
        this.Expiry = Expiry ?? throw new ArgumentNullException(nameof(Expiry));
    }
}
