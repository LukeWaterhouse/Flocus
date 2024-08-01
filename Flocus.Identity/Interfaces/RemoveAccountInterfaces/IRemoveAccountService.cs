using Flocus.Identity.Models;

namespace Flocus.Identity.Interfaces;

public interface IRemoveAccountService
{
    Task DeleteUserAsUserAsync(string username, string password);
    Task DeleteUserAsAdminAsync(string username);
    Task DeleteAdminAsAdminAsync(string username, string password);
    Task DeleteAdminAsAdminWithKeyAsync(string username, string key);
}
