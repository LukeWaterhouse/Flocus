namespace Flocus.Domain.Interfacesl;

public interface IRepositoryService
{
    Task CreateDbUserAsync(string username, string passwordHash, bool AdminRights);
}
