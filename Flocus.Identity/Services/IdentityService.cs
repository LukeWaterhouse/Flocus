using Flocus.Domain.Interfacesl;
using Flocus.Identity.Interfaces;
using Flocus.Repository.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace Flocus.Identity.Services;

public class IdentityService : IIdentityService
{
    private readonly IRepositoryService _repositoryServiceMock;
    private readonly string? _adminKey;
    private readonly string? _signingKey;

    public IdentityService(IRepositoryService repositoryService, IConfiguration configuration)
    {
        _repositoryServiceMock = repositoryService;
        _signingKey = configuration.GetSection("AppSettings")["SigningKey"];
        _adminKey = configuration.GetSection("AppSettings")["AdminKey"];

    }

    public async Task RegisterAsync(string username, string password, string emailAddress, bool isAdmin, string? key)
    {
        if (isAdmin && _adminKey != key)
        {
            throw new AuthenticationException("Key was incorrect.");
        }

        string passwordHash = BC.HashPassword(password);
        var isSuccessful = await _repositoryServiceMock.CreateDbUserAsync(username, passwordHash, emailAddress, isAdmin);

        if (!isSuccessful) { throw new Exception("There was an error when creating the user."); }
    }

    public async Task<string> GetAuthTokenAsync(string username, string password)
    {
        try
        {
            var user = await _repositoryServiceMock.GetUserAsync(username);
            var isVerified = BC.Verify(password, user.PasswordHash);

            if (isVerified)
            {
                return GenerateToken(username);
            }
            throw new AuthenticationException("Invalid username and password combination");

        }
        catch (RecordNotFoundException)
        {
            throw new AuthenticationException("Invalid username and password combination");
        }
    }

    private string GenerateToken(string username)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username)
        };
        if (_signingKey == null || _signingKey == "")
        {
            throw new Exception("invalid signing key from config");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signingKey));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
