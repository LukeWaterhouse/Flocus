using Npgsql;

namespace Flocus.Repository.Interfaces;

public interface IDbConnectionService
{
    NpgsqlConnection CreateDbConnection();
}
