namespace Flocus.Domain.Interfaces;

public interface IUserService
{
    Task RegisterAsync(string username, string password, bool isAdmin, string? key);
    Task GetAuthTokenAsync(string username, string password);
}
