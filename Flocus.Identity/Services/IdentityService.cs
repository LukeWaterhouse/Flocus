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
    private readonly IdentitySettings _appSettings;

    public IdentityService(IRepositoryService repositoryService, IdentitySettings appSettings)
    {
        _repositoryService = repositoryService;
        _appSettings = appSettings;
    }

    public async Task RegisterAsync(string username, string password, string emailAddress, bool isAdmin, string? key)
    {
        if (isAdmin && _appSettings.AdminKey != key)
        {
            throw new AuthenticationException("Key was incorrect.");
        }

        string passwordHash = BC.HashPassword(password);
        var isSuccessful = await _repositoryService.CreateDbUserAsync(username, passwordHash, emailAddress, isAdmin);

        if (!isSuccessful) { throw new Exception("There was an error when creating the user."); }
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
            throw new AuthenticationException("Invalid username and password combination");

        }
        catch (RecordNotFoundException)
        {
            throw new AuthenticationException("Invalid username and password combination");
        }
    }

    // I think just this should have authorization
    public async Task DeleteUser(string username, string password)
    {
        var user = await _repositoryService.GetUserAsync(username);
        var isVerified = BC.Verify(password, user.PasswordHash);

        if (isVerified)
        {
            var isUserDeleted = await _repositoryService.DeleteUser(user.ClientId);
            if (!isUserDeleted)
            {
                throw new Exception("There was an issue deleting the user");
            }
        }
        else
        {
            throw new AuthenticationException("Invalid username and password combination");
        }
    }

    #region HelperMethods
    private string GenerateToken(User user)
    {
        var role = user.IsAdmin ? "Admin" : "User";

        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.EmailAddress),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.SigningKey));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //add issuer and audience here
        var token = new JwtSecurityToken(
            issuer: _appSettings.Issuer,
            audience: _appSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    #endregion
}
