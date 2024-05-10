using Flocus.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Flocus.Repository.Services;

public class DbConnectionService : IDbConnectionService
{
    private readonly string _connectionString;

    public DbConnectionService(IConfiguration configuration)
    {
        _connectionString = configuration.GetSection("ConnectionStrings")["FlocusDb"];
    }
    public NpgsqlConnection CreateDbConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
