using Flocus.Domain.Models;

namespace Flocus.Domain.Interfacesl;

public interface IRepositoryService
{
    Task CreateDbUserAsync(string username, string passwordHash, string emailAddress, bool adminRights);
    Task<User> GetUserAsync(string username);
    Task DeleteUser(string userId);
}
