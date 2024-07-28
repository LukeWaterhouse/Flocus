using AutoMapper;
using Flocus.Domain.Interfaces;
using Flocus.Domain.Models;
using Flocus.Repository.Exceptions;
using Flocus.Repository.Interfaces;
using Flocus.Repository.Models;

namespace Flocus.Repository.Services;

public class UserRepositoryService : IUserRepositoryService
{

    private readonly IMapper _mapper;
    private readonly IUserSqlService _sqlQueryService;

    public UserRepositoryService(
        IMapper mapper,
        IUserSqlService sqlQueryFactory)
    {
        _mapper = mapper;
        _sqlQueryService = sqlQueryFactory;
    }

    public async Task CreateDbUserAsync(string username, string passwordHash, string emailAddress, bool adminRights)
    {
        await ExistingUserGuard(username, emailAddress);

        var dbUserToCreate = new DbUser(
            Guid.NewGuid().ToString(),
            emailAddress,
            DateTime.UtcNow,
            username,
            passwordHash,
            adminRights);
        var success = await _sqlQueryService.CreateUserAsync(dbUserToCreate);
        if (!success) { throw new Exception("There was an error when creating the user."); }
    }

    public async Task DeleteUser(string userId)
    {
        var success = await _sqlQueryService.DeleteUserWithRelatedTables(userId);
        if (!success) { throw new Exception("There was an error when deleting the user."); }
    }

    public async Task<User> GetUserAsync(string username)
    {
        var dbUserList = await _sqlQueryService.GetUsersByUsernameAsync(username);

        if (dbUserList.Count == 0)
        {
            throw new RecordNotFoundException($"No user could be found with username: {username}");
        }

        if (dbUserList.Count != 1)
        {
            throw new Exception($"invalid number of users with username: {username}, found {dbUserList.Count}");
        }

        var user = _mapper.Map<User>(dbUserList.FirstOrDefault());
        return user;
    }

    #region HelperMethods
    private async Task ExistingUserGuard(string username, string emailAddress)
    {
        var dbUserList = await _sqlQueryService.GetUsersByUsernameOrEmailAsync(username, emailAddress);

        var emailExists = false;
        var userNameExists = false;

        if (dbUserList.Count > 0)
        {
            foreach (var dbUser in dbUserList)
            {
                if (dbUser.Username == username)
                {
                    userNameExists = true;
                }

                if (dbUser.Email_address == emailAddress)
                {
                    emailExists = true;
                }
            }

            if (userNameExists)
            {
                //TODO: think about changing user in message to admin if admin
                throw new DuplicateRecordException($"user already exists with username: {username}");
            }

            if (emailExists)
            {
                throw new DuplicateRecordException($"user already exists with email: {emailAddress}");
            }

            throw new Exception($"Results found when searching users by {username} or {emailAddress} but were not caught.");
        }
    }
    #endregion
}
