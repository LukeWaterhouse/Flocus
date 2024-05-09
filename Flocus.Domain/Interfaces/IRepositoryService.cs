using Flocus.Domain.Models;

namespace Flocus.Domain.Interfacesl;

public interface IRepositoryService
{
    Task<bool> CreateDbUserAsync(string username, string passwordHash, bool adminRights);
    Task<User> GetUserAsync(string username);
}
