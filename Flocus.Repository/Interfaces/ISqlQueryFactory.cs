using Flocus.Repository.Models;
using Npgsql;

namespace Flocus.Repository.Interfaces;

internal interface ISqlQueryFactory
{
    string GenerateInsertClientQuery(string username, string passwordHash, bool AdminRights);

    string GenerateGetUserQuery(string username);
}
