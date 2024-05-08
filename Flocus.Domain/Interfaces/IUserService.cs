namespace Flocus.Domain.Interfaces;

public interface IUserService
{
    Task CreateUserAsync(string username, string password, bool isAdmin, string? key);
    Task LoginUserAsync(string username, string password);
}
