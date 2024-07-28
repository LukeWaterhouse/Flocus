using Flocus.Domain.Interfaces;
using Flocus.Domain.Models;
using Flocus.Identity.Interfaces.AuthTokenInterfaces;
using Flocus.Identity.Models;
using Flocus.Repository.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace Flocus.Identity.Services.AuthTokenServices;

public class AuthTokenService : IAuthTokenService
{
    private readonly IUserRepositoryService _userRepositoryService;
    private readonly IdentitySettings _identitySettings;

    private readonly string IncorrectPasswordMessage = "Incorrect username and password combination";

    public AuthTokenService(IUserRepositoryService userRepositoryService, IdentitySettings identitySettings)
    {
        _userRepositoryService = userRepositoryService;
        _identitySettings = identitySettings;
    }

    public async Task<string> GetAuthTokenAsync(string username, string password)
    {
        try
        {
            var user = await _userRepositoryService.GetUserAsync(username);
            var isVerified = BC.Verify(password, user.PasswordHash);

            if (isVerified)
            {
                return GenerateToken(user);
            }
            throw new AuthenticationException(IncorrectPasswordMessage);

        }
        catch (RecordNotFoundException)
        {
            throw new AuthenticationException(IncorrectPasswordMessage);
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
