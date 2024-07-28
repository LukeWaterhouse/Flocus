namespace Flocus.Identity.Interfaces.AuthTokenInterfaces;

public interface IAuthTokenService
{
    Task<string> GetAuthTokenAsync(string username, string password);
}
