using AutoMapper;
using Flocus.Domain.Interfacesl;
using Flocus.Domain.Models;
using Flocus.Repository.Exceptions;
using Flocus.Repository.Interfaces;
using Flocus.Repository.Models;

namespace Flocus.Repository.Services;

public class RepositoryService : IRepositoryService
{

    private readonly IMapper _mapper;
    private readonly IDbConnectionService _dbConnectionService;
    private readonly ISqlQueryService _sqlQueryService;

    public RepositoryService(
        IMapper mapper,
        IDbConnectionService dbConnectionFactory,
        ISqlQueryService sqlQueryFactory)
    {
        _mapper = mapper;
        _dbConnectionService = dbConnectionFactory;
        _sqlQueryService = sqlQueryFactory;
    }

    public async Task<bool> CreateDbUserAsync(string username, string passwordHash, string emailAddress, bool adminRights)
    {
        var dbUserList = await _sqlQueryService.GetUsersByUsernameAsync(username);
        if (dbUserList.Count > 0)
        {
            throw new DuplicateRecordException($"user already exists with username: {username}");
        }

        var dbUserToCreate = new DbUser
        {
            Client_id = Guid.NewGuid().ToString(),
            Email_address = emailAddress,
            Account_creation_date = DateTime.Now,
            Username = username,
            Password_hash = passwordHash,
            Admin_rights = adminRights
        };

        var success = await _sqlQueryService.CreateUserAsync(dbUserToCreate);
        return success;
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

        var user = _mapper.Map<User>(dbUserList[0]);
        return user;
    }
}
