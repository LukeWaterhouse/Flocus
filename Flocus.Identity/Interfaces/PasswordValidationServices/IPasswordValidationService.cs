namespace Flocus.Identity.Interfaces.PasswordValidationServices;

public interface IPasswordValidationService
{
    string IncorrectUsernamePasswordMessage { get; }
    public void ValidatePassword(string password, string passwordHash);
}
