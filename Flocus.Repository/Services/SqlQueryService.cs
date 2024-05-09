using Flocus.Repository.Interfaces;

namespace Flocus.Repository.Services;

internal class SqlQueryService : ISqlQueryFactory
{
    public string GenerateGetUserQuery(string username)
    {
        var query = $"SELECT * FROM public.client WHERE username='{username}'";
        return query;
    }

    public string GenerateInsertClientQuery(string username, string passwordHash, bool adminRights)
    {
        var clientId = Guid.NewGuid();
        var creationDate = DateTime.Now;

        var query = $"INSERT INTO public.client (client_id, profile_picture, account_creation_date, username, password_hash, admin_rights) " +
            $"VALUES ('{clientId}', 'my profile picture', '{creationDate}', '{username}', '{passwordHash}', {adminRights});";

        return query;
    }
}
