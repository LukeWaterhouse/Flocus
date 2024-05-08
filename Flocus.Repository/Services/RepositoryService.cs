using AutoMapper;
using Dapper;
using Flocus.Domain.Interfacesl;
using Flocus.Domain.Models;
using Flocus.Repository.Interfaces;
using Flocus.Repository.Models;
using Npgsql;

namespace Flocus.Repository.Services;

internal class RepositoryService : IRepositoryService
{

    private readonly IMapper _mapper;
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public RepositoryService(IMapper mapper, IDbConnectionFactory dbConnectionFactory)
    {
        _mapper = mapper;
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task CreateDbUserAsync(string username, string passwordHash, bool AdminRights)
    {
        var connString = "Host=localhost;Port=5432;Database=flocusdb;Username=myuser;Password=mypassword;";
        var clientId = Guid.NewGuid();
        var creationDate = DateTime.Now;

        await using (var conn = _dbConnectionFactory.CreateNpgSqlConnection())
        {
            await conn.OpenAsync();
            await using (var cmd = new NpgsqlCommand($"INSERT INTO public.client (client_id, profile_picture, account_creation_date, username, password_hash, password_salt, admin_rights)\n    VALUES ({clientId}, 'my profile picture', {creationDate}, {username}, {passwordHash}, {AdminRights});", conn))

        }

        await using (var cmd = new NpgsqlCommand($"INSERT INTO public.client (client_id, profile_picture, account_creation_date, username, password_hash, password_salt, admin_rights)\n    VALUES ({clientId}, 'my profile picture', {creationDate}, {username}, {passwordHash}, {AdminRights});", conn))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
                Console.WriteLine(reader.GetString(0));
        }
    }

    public async Task<User> GetUserAsync(string username)
    {
        var connString = "Host=localhost;Port=5432;Database=flocusdb;Username=myuser;Password=mypassword;";

        await using var conn = new NpgsqlConnection(connString);
        var DbUserList = conn.Query<DbUser>($"SELECT * FROM public.client WHERE username='{username}'").ToList();
        var user = _mapper.Map<User>(DbUserList[0]);
        return user;
    }
}
