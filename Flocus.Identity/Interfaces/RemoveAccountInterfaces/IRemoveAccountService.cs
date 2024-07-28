using Flocus.Identity.Models;

namespace Flocus.Identity.Interfaces;

public interface IRemoveAccountService
{
    Task DeleteUserAsUser(string username, string password);
    Task DeleteUserAsAdmin(string username);

    Task DeleteAdminAsAdmin(string username, string password);
    Task DeleteAdminAsAdminWithKey(string username, string key);
}
