namespace Flocus.Identity.Interfaces;

public interface IIdentityService
{
    Task RegisterAsync(string username, string password, bool isAdmin, string? key);
    Task<string> GetAuthTokenAsync(string username, string password);
}
