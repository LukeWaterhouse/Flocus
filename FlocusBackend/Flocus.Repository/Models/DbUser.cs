namespace Flocus.Repository.Models;

public sealed record DbUser(
    string Client_id,
    string Email_address,
    DateTime Account_creation_date,
    string Username,
    string Password_hash,
    bool Admin_rights);