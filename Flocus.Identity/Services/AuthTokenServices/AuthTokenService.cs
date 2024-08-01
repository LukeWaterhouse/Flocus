using Flocus.Domain.Interfaces;
using Flocus.Domain.Models;
using Flocus.Identity.Interfaces.AuthTokenInterfaces;
using Flocus.Identity.Interfaces.PasswordValidationServices;
using Flocus.Identity.Models;
using Flocus.Repository.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;

namespace Flocus.Identity.Services.AuthTokenServices;

public class AuthTokenService : IAuthTokenService
{
    private readonly IUserRepositoryService _userRepositoryService;
    private readonly IPasswordValidationService _passwordValidationService;
    private readonly IdentitySettings _identitySettings;

    public AuthTokenService(
        IUserRepositoryService userRepositoryService,
        IPasswordValidationService passwordValidationService,
        IdentitySettings identitySettings)
    {
        _userRepositoryService = userRepositoryService;
        _passwordValidationService = passwordValidationService;
        _identitySettings = identitySettings;
    }

    public async Task<string> GetAuthTokenAsync(string username, string password)
    {
        try
        {
            var user = await _userRepositoryService.GetUserAsync(username);
            _passwordValidationService.ValidatePassword(password, user.PasswordHash);
            return GenerateToken(user);
        }
        catch (RecordNotFoundException)
        {
            throw new AuthenticationException(_passwordValidationService.IncorrectUsernamePasswordMessage);
        }
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
}
