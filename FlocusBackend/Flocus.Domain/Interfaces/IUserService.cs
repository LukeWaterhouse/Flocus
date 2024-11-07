using Flocus.Domain.Models;

namespace Flocus.Domain.Interfaces;

public interface IUserService
{
    Task<User> GetUserAsync(string username);
}
