namespace Flocus.Identity.Interfaces.PasswordValidationServices;

public interface IPasswordValidationService
{
    string InvalidUsernamePasswordMessage { get; }
    public void ValidatePassword(string password, string passwordHash);
}
