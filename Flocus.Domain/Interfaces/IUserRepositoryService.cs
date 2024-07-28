using Flocus.Domain.Models;

namespace Flocus.Domain.Interfaces;

public interface IUserRepositoryService
{
    Task CreateDbUserAsync(string username, string passwordHash, string emailAddress, bool adminRights);
    Task<User> GetUserAsync(string username);
    Task DeleteUser(string userId);
}
