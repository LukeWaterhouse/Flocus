namespace Flocus.Identity.Models;

public sealed class IdentitySettings
{
    public string SigningKey { get; }
    public string Issuer { get; }
    public string Audience { get; }
    public string AdminKey { get; }

    public IdentitySettings(string? signingKey, string? issuer, string? audience, string? adminKey)
    {
        SigningKey = string.IsNullOrEmpty(signingKey) ? throw new ArgumentNullException(nameof(signingKey)) : signingKey;
        Issuer = string.IsNullOrEmpty(issuer) ? throw new ArgumentNullException(nameof(issuer)) : issuer;
        Audience = string.IsNullOrEmpty(audience) ? throw new ArgumentNullException(nameof(audience)) : audience;
        AdminKey = string.IsNullOrEmpty(adminKey) ? throw new ArgumentNullException(nameof(adminKey)) : adminKey;
    }
}
