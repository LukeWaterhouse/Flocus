using Flocus.Domain.Interfaces;
using Flocus.Domain.Models;
using Flocus.Identity.Interfaces.AdminKeyInterfaces;
using Flocus.Identity.Interfaces.PasswordValidationServices;
using Flocus.Identity.Interfaces.RemoveAccountInterfaces;

namespace Flocus.Identity.Services.RemoveAccountServices;


//TOOD: this needs a redesign, and move the controller logic into here too
public class RemoveAccountService : IRemoveAccountService
{
    private readonly IUserRepositoryService _userRepositoryService;
    private readonly IAdminKeyService _checkAdminKeyService;
    private readonly IPasswordValidationService _passwordValidationServiceMock;

    public RemoveAccountService(
        IUserRepositoryService userRepositoryService,
        IAdminKeyService checkAdminKeyService,
        IPasswordValidationService passwordValidationService)
    {
        _userRepositoryService = userRepositoryService;
        _checkAdminKeyService = checkAdminKeyService;
        _passwordValidationServiceMock = passwordValidationService;
    }

    public async Task DeleteSelfUserAsync(string username, string password)
    {
        var user = await _userRepositoryService.GetUserAsync(username);
        await VerifyAndDeleteUser(user, password);
    }

    public async Task DeleteUserByNameAsync(string username, string? adminKey)
    {
        var user = await _userRepositoryService.GetUserAsync(username);

        if (!user.IsAdmin)
        {
            await DeleteUser(user);
            return;
        }

        if (adminKey == null)
        {
            throw new UnauthorizedAccessException($"You must provide an admin key when deleting a different admin account: {username}");
        }

        _checkAdminKeyService.CheckAdminKeyCorrect(adminKey);
        await DeleteUser(user);
    }

    #region Private Methods
    private async Task DeleteUser(User user)
    {
        await _userRepositoryService.DeleteUserAsync(user.ClientId);
    }

    private async Task VerifyAndDeleteUser(User user, string password)
    {
        _passwordValidationServiceMock.ValidatePassword(password, user.PasswordHash);
        await DeleteUser(user);
    }
    #endregion
}
