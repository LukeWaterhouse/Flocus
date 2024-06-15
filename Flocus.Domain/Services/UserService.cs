using Flocus.Domain.Interfaces;
using Flocus.Domain.Interfacesl;
using Flocus.Domain.Models;

namespace Flocus.Domain.Services;

public class UserService : IUserService
{
    private readonly IRepositoryService _repositoryService;

    public UserService(IRepositoryService repositoryService)
    {
        _repositoryService = repositoryService;
    }

    public async Task<User> GetUserAsync(string username) // This should come from the token
    {
        var user = await _repositoryService.GetUserAsync(username);
        return user;
    }
}
