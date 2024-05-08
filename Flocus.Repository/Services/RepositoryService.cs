using Flocus.Domain.Interfacesl;
using Npgsql;

namespace Flocus.Repository.Services;

internal class RepositoryService : IRepositoryService
{

    public async Task CreateDbUserAsync(string username, string passwordHash, bool AdminRights)
    {
        var connString = "Host=localhost;Port=5432;Database=mydatabase;Username=myuser;Password=mypassword;";

        await using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync();

       /* await using var command = dataSource.CreateCommand("SELECT * clients FROM flocusdb");
        await using var reader = await command.ExecuteReaderAsync();*/

/*        while (await reader.ReadAsync())
        {
            Console.WriteLine(reader.GetString(0));
        }*/
    }
}
