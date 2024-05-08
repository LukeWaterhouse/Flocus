namespace Flocus.Repository.Models;

internal class DbUser
{
    public string Id { get; set; }
    public DateTime CreationDate { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public bool AdminRights { get; set; }
}
