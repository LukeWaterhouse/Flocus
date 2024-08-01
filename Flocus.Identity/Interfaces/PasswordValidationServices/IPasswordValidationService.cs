namespace Flocus.Identity.Interfaces.PasswordValidationServices;

public interface IPasswordValidationService
{
    string IncorrectPasswordMessage { get; }
    public void ValidatePassword(string password, string passwordHash);
}
