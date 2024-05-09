using Flocus.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Flocus.Repository.Services;

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetSection("ConnectionStrings")["FlocusDb"];
    }
    public NpgsqlConnection CreateNpgSqlConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
