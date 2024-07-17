using Flocus.Domain.Interfacesl;
using Flocus.Domain.Models;
using Flocus.Identity.Interfaces;
using Flocus.Identity.Models;
using Flocus.Repository.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace Flocus.Identity.Services;

public class IdentityService : IIdentityService
{
    private readonly IRepositoryService _repositoryService;
    private readonly IRegisterValidationService _registerValidationService;
    private readonly IdentitySettings _identitySettings;

    private readonly string InvalidPasswordMessage = "Invalid username and password combination";
    private readonly string AdminKeyIncorrectMessage = "Admin key is not correct";
    private readonly string CannotDeleteAdminUserMessage = "Cannot delete admin user";

    public IdentityService(IRepositoryService repositoryService, IRegisterValidationService registerValidationService, IdentitySettings identitySettings)
    {
        _repositoryService = repositoryService;
        _identitySettings = identitySettings;
        _registerValidationService = registerValidationService;
    }

    public async Task RegisterAsync(RegistrationModel registrationModel)
    {
        _registerValidationService.ValidateRegistrationModel(registrationModel);

        if (registrationModel.IsAdmin)
        {
            CheckAdminKeyCorrect(registrationModel.Key);
        }

        string passwordHash = BC.HashPassword(registrationModel.Password);
        await _repositoryService.CreateDbUserAsync(registrationModel.Username, passwordHash, registrationModel.EmailAddress, registrationModel.IsAdmin);
    }

    public async Task<string> GetAuthTokenAsync(string username, string password)
    {
        try
        {
            var user = await _repositoryService.GetUserAsync(username);
            var isVerified = BC.Verify(password, user.PasswordHash);

            if (isVerified)
            {
                return GenerateToken(user);
            }
            throw new AuthenticationException(InvalidPasswordMessage);

        }
        catch (RecordNotFoundException)
        {
            throw new AuthenticationException(InvalidPasswordMessage);
        }
    }

    public async Task DeleteUserAsUser(string username, string password)
    {
        var user = await _repositoryService.GetUserAsync(username);
        EnsureUserNotAdmin(user);
        await VerifyAndDeleteUser(user, password);
    }


    public async Task DeleteUserAsAdmin(string username)
    {
        var user = await _repositoryService.GetUserAsync(username);
        EnsureUserNotAdmin(user);
        await DeleteUser(user);
    }

    public async Task DeleteAdminAsAdmin(string username, string password)
    {
        var adminUser = await _repositoryService.GetUserAsync(username);
        await VerifyAndDeleteUser(adminUser, password);
    }

    public async Task DeleteAdminAsAdminWithKey(string username, string key)
    {
        var user = await _repositoryService.GetUserAsync(username);
        CheckAdminKeyCorrect(key);
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
        await _repositoryService.DeleteUser(user.ClientId);
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

    private string GenerateToken(User user)
    {
        var role = user.IsAdmin ? "Admin" : "User";

        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.EmailAddress),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_identitySettings.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _identitySettings.Issuer,
            audience: _identitySettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private void CheckAdminKeyCorrect(string? adminKey)
    {
        if (adminKey != _identitySettings.AdminKey)
        {
            throw new AuthenticationException(AdminKeyIncorrectMessage);
        }
    }
    #endregion
}
