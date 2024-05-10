namespace Flocus.Repository.Models;

public class DbUser
{
    public string Client_id { get; set; }
    public string Email_address { get; set; }
    public DateTime Account_creation_date { get; set; }
    public string Username { get; set; }
    public string Password_hash { get; set; }
    public bool Admin_rights { get; set; }
}
