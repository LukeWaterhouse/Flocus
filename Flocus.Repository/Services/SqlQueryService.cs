using Dapper;
using Flocus.Repository.Interfaces;
using Flocus.Repository.Models;
using Npgsql;
using System.Diagnostics.CodeAnalysis;

namespace Flocus.Repository.Services;

// Mocking is very awkward here just do sanity testing and integration tests to cover this.
// The methods here should have as little logic as possible.
[ExcludeFromCodeCoverage]
public class SqlQueryService : ISqlQueryService
{
    private readonly IDbConnectionService _dbConnectionService;

    public SqlQueryService(IDbConnectionService dbConnectionService)
    {
        _dbConnectionService = dbConnectionService;
    }

    public async Task<List<DbUser>> GetUsersByUsernameAsync(string username)
    {
        await using (var conn = _dbConnectionService.CreateDbConnection())
        {
            var query = $"SELECT * FROM public.client WHERE username='{username}'";
            var dbUserList = await conn.QueryAsync<DbUser>(query);

            return dbUserList.ToList();
        }
    }

    public async Task<bool> CreateUserAsync(DbUser dbUser)
    {
        await using (var conn = _dbConnectionService.CreateDbConnection())
        {
            await conn.OpenAsync();

            var query = $"INSERT INTO public.client (client_id, email_address, account_creation_date, username, password_hash, admin_rights) " +
                $"VALUES ('{dbUser.Client_id}', '{dbUser.Email_address}', '{dbUser.Account_creation_date}', '{dbUser.Username}', '{dbUser.Password_hash}', {dbUser.Admin_rights});";

            var affectedRows = 0;
            await using (var cmd = new NpgsqlCommand(query, conn))
            {
                affectedRows = await cmd.ExecuteNonQueryAsync();
            }

            return affectedRows > 0;
        }
    }
}
