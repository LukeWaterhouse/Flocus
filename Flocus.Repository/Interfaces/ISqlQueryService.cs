using Flocus.Repository.Models;

namespace Flocus.Repository.Interfaces;

public interface ISqlQueryService
{
    Task<bool> CreateUserAsync(DbUser dbUser);

    Task<List<DbUser>> GetUsersByUsernameAsync(string username);
}
