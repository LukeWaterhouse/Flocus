using Flocus.Identity.Interfaces.PasswordValidationServices;
using System.Security.Authentication;
using BC = BCrypt.Net.BCrypt;

namespace Flocus.Identity.Services.PasswordValidationServices;

public sealed class PasswordValidationService : IPasswordValidationService
{
    string IPasswordValidationService.IncorrectPasswordMessage => IncorrectPasswordMessage;
    public readonly string IncorrectPasswordMessage = "Invalid username and password combination";

    public void ValidatePassword(string password, string passwordHash)
    {
        var isVerified = BC.Verify(password, passwordHash);
        if (!isVerified)
        {
            throw new AuthenticationException(IncorrectPasswordMessage);
        }
    }
}
