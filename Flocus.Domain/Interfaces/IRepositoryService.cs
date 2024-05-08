using Flocus.Domain.Models;

namespace Flocus.Domain.Interfacesl;

public interface IRepositoryService
{
    Task CreateDbUserAsync(string username, string passwordHash, bool AdminRights);
    Task<User> GetUserAsync(string username);
}
