using Flocus.Domain.Interfaces;
using Flocus.Domain.Models;
using Flocus.Identity.Interfaces;
using Flocus.Identity.Interfaces.AdminKeyInterfaces;
using System.Security.Authentication;
using BC = BCrypt.Net.BCrypt;

namespace Flocus.Identity.Services;

public class RemoveAccountService : IRemoveAccountService
{
    private readonly IUserRepositoryService _userRepositoryService;
    private readonly IAdminKeyService _checkAdminKeyService;

    private readonly string InvalidPasswordMessage = "Invalid username and password combination";
    private readonly string CannotDeleteAdminUserMessage = "Cannot delete admin user";

    public RemoveAccountService(IUserRepositoryService userRepositoryService, IAdminKeyService checkAdminKeyService)
    {
        _userRepositoryService = userRepositoryService;
        _checkAdminKeyService = checkAdminKeyService;
    }

    public async Task DeleteUserAsUser(string username, string password)
    {
        var user = await _userRepositoryService.GetUserAsync(username);
        EnsureUserNotAdmin(user);
        await VerifyAndDeleteUser(user, password);
    }

    public async Task DeleteUserAsAdmin(string username)
    {
        //TODO: need to check admin still exists or a rampant deleted admin with a token still valid can continue to do damage.
        var user = await _userRepositoryService.GetUserAsync(username);
        EnsureUserNotAdmin(user);
        await DeleteUser(user);
    }

    public async Task DeleteAdminAsAdmin(string username, string password)
    {
        var adminUser = await _userRepositoryService.GetUserAsync(username);
        await VerifyAndDeleteUser(adminUser, password);
    }

    public async Task DeleteAdminAsAdminWithKey(string username, string key)
    {
        var user = await _userRepositoryService.GetUserAsync(username);
        _checkAdminKeyService.CheckAdminKeyCorrect(key);
        await DeleteUser(user);
    }

    #region HelperMethods
    private void EnsureUserNotAdmin(User user)
    {
        if (user.IsAdmin)
        {
            throw new UnauthorizedAccessException(CannotDeleteAdminUserMessage);
        }
    }

    private async Task DeleteUser(User user)
    {
        await _userRepositoryService.DeleteUser(user.ClientId);
    }

    private async Task VerifyAndDeleteUser(User user, string password)
    {
        var isVerified = BC.Verify(password, user.PasswordHash);
        if (isVerified)
        {
            await DeleteUser(user);
            return;
        }
        throw new AuthenticationException(InvalidPasswordMessage);
    }
    #endregion
}
