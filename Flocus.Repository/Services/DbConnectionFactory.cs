using Flocus.Repository.Interfaces;
using Npgsql;

namespace Flocus.Repository.Services;

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(string dbConnectionString)
    {
        _connectionString = dbConnectionString;

    }
    public NpgsqlConnection CreateNpgSqlConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
