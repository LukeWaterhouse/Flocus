using Flocus.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Flocus.Repository.Services;

public class DbConnectionService : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionService(IConfiguration configuration)
    {
        _connectionString = configuration.GetSection("ConnectionStrings")["FlocusDb"];
    }
    public NpgsqlConnection CreateNpgSqlConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
