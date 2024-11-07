namespace Flocus.Identity.Interfaces.RemoveAccountInterfaces;

public interface IRemoveAccountService
{
    Task DeleteSelfUserAsync(string username, string password);
    Task DeleteUserByNameAsync(string username, string? adminKey);
}
