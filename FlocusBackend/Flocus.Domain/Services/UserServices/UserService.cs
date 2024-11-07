using Flocus.Domain.Interfaces;
using Flocus.Domain.Models;

namespace Flocus.Domain.Services.UserServices;

public sealed class UserService : IUserService
{
    private readonly IUserRepositoryService _repositoryService;

    public UserService(IUserRepositoryService repositoryService)
    {
        _repositoryService = repositoryService;
    }

    public async Task<User> GetUserAsync(string username)
    {
        var user = await _repositoryService.GetUserAsync(username);
        return user;
    }
}
