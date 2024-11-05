namespace Flocus.Models.ReturnModels.UserReturnModels;

public record UserSensitiveInfoDto(
    string ClientId, 
    string EmailAddress, 
    DateTime CreatedAt, 
    string Username, 
    bool IsAdmin)
    : UserBasicInfoDto(CreatedAt, Username);
