using Flocus.Repository.Models;

namespace Flocus.Repository.Interfaces;

public interface ISqlQueryService
{
    Task<bool> CreateUserAsync(DbUser dbUser);

    Task<List<DbUser>> GetUsersByUsernameAsync(string username);

    Task<List<DbUser>> GetUsersByUsernameOrEmailAsync(string username, string email);

    Task<bool> DeleteUserWithRelatedTables(string username);
}
