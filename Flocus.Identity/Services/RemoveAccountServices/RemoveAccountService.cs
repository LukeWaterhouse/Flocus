using Flocus.Domain.Interfaces;
using Flocus.Domain.Models;
using Flocus.Identity.Interfaces;
using Flocus.Identity.Interfaces.AdminKeyInterfaces;
using Flocus.Identity.Interfaces.PasswordValidationServices;

namespace Flocus.Identity.Services.RemoveAccountServices;


//TOOD: this needs a redesign, and move the controller logic into here too
public class RemoveAccountService : IRemoveAccountService
{
    private readonly IUserRepositoryService _userRepositoryService;
    private readonly IAdminKeyService _checkAdminKeyService;
    private readonly IPasswordValidationService _passwordValidationServiceMock;

    private readonly string CannotDeleteAdminUserMessage = "Cannot delete admin user";

    public RemoveAccountService(
        IUserRepositoryService userRepositoryService,
        IAdminKeyService checkAdminKeyService,
        IPasswordValidationService passwordValidationService)
    {
        _userRepositoryService = userRepositoryService;
        _checkAdminKeyService = checkAdminKeyService;
        _passwordValidationServiceMock = passwordValidationService;
    }

    public async Task DeleteUserAsUserAsync(string username, string password)
    {
        var user = await _userRepositoryService.GetUserAsync(username);
        EnsureUserNotAdmin(user);
        await VerifyAndDeleteUser(user, password);
    }

    public async Task DeleteUserAsAdminAsync(string username)
    {
        var user = await _userRepositoryService.GetUserAsync(username);
        EnsureUserNotAdmin(user);
        await DeleteUser(user);
    }

    public async Task DeleteAdminAsAdminAsync(string username, string password)
    {
        var adminUser = await _userRepositoryService.GetUserAsync(username);
        await VerifyAndDeleteUser(adminUser, password);
    }

    public async Task DeleteAdminAsAdminWithKeyAsync(string username, string key)
    {
        //TODO: need to check admin still exists or a rampant deleted admin with a token still valid can continue to do damage.
        var user = await _userRepositoryService.GetUserAsync(username);
        _checkAdminKeyService.CheckAdminKeyCorrect(key);
        await DeleteUser(user);
    }

    #region Private Methods
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
        _passwordValidationServiceMock.ValidatePassword(password, user.PasswordHash);
        await DeleteUser(user);
    }
    #endregion
}
