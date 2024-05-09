using AutoMapper;
using Dapper;
using Flocus.Domain.Interfacesl;
using Flocus.Domain.Models;
using Flocus.Repository.Exceptions;
using Flocus.Repository.Interfaces;
using Flocus.Repository.Models;
using Npgsql;

namespace Flocus.Repository.Services;

internal class RepositoryService : IRepositoryService
{

    private readonly IMapper _mapper;
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly ISqlQueryFactory _sqlQueryFactory;

    public RepositoryService(
        IMapper mapper,
        IDbConnectionFactory dbConnectionFactory,
        ISqlQueryFactory sqlQueryFactory)
    {
        _mapper = mapper;
        _dbConnectionFactory = dbConnectionFactory;
        _sqlQueryFactory = sqlQueryFactory;
    }

    public async Task<bool> CreateDbUserAsync(string username, string passwordHash, bool adminRights)
    {

        int affectedRows = 0;

        await using (var conn = _dbConnectionFactory.CreateNpgSqlConnection())
        {

            var DbUserList = conn.Query<DbUser>(_sqlQueryFactory.GenerateGetUserQuery(username)).ToList();
            if(DbUserList.Count > 0)
            {
                throw new DuplicateRecordException($"user already exists with username: {username}");
            }

            await conn.OpenAsync();
            await using (var cmd = new NpgsqlCommand(_sqlQueryFactory.GenerateInsertClientQuery(username, passwordHash, adminRights), conn))
            {
                affectedRows = await cmd.ExecuteNonQueryAsync();
            }
        }

        if (affectedRows > 0)
        {
            return true;
        }

        return false;
    }

    public async Task<User> GetUserAsync(string username)
    {
        await using (var conn = _dbConnectionFactory.CreateNpgSqlConnection())
        {
            var DbUserList = conn.Query<DbUser>(_sqlQueryFactory.GenerateGetUserQuery(username)).ToList();

            if(DbUserList.Count == 0)
            {
                throw new RecordNotFoundException($"No user could be found with username: {username}");
            }

            if (DbUserList.Count != 1)
            {
                throw new Exception($"invalid number of users with username: {username}, found {DbUserList.Count}");
            }

            var user = _mapper.Map<User>(DbUserList[0]);
            return user;
        };
    }
}
