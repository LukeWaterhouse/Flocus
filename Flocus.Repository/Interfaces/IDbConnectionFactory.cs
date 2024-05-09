using Npgsql;

namespace Flocus.Repository.Interfaces;

internal interface IDbConnectionFactory
{
    NpgsqlConnection CreateNpgSqlConnection();
}
