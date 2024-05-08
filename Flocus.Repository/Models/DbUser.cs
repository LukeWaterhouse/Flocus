namespace Flocus.Repository.Models;

internal class DbUser
{
    public string Client_id { get; set; }
    public string Profile_picture { get; set; }
    public DateTime Account_creation_date { get; set; }
    public string Username { get; set; }
    public string Password_hash { get; set; }
    public bool Admin_rights { get; set; }
}
