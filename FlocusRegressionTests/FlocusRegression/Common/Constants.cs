using System.Text.Json;

namespace FlocusRegressionTests.Common;

public static class Constants
{
    //Base Url
    public static readonly string BaseUrl = "https://localhost:7207";

    //Controllers
    public static readonly string IdentityController = "Identity";
    public static readonly string UserController = "User";

    //URL Segments
    public static readonly string GetTokenSegment = IdentityController + "/token";
    public static readonly string RegisterSegment = IdentityController + "/register";
    public static readonly string DeleteSelfSegment = IdentityController + "/user";
    public static readonly string DeleteByNameSegmentTemplate = IdentityController + "/admin/user/{0}";

    public static readonly string GetUserSegment = UserController;
    public static readonly string GetUserAsAdminSegmentTemplate = UserController + "/admin/user/{0}";

    //Request Form Keys
    public static readonly string UsernameRequestKey = "username";
    public static readonly string PasswordRequestKey = "password";
    public static readonly string EmailAddressRequestKey = "emailAddress";
    public static readonly string IsAdminRequestKey = "isAdmin";
    public static readonly string AdminKeyRequestKey = "key";

    //Test Collections
    public static readonly string UserRegistration = "UserRegistration";

    //JsonSerializerOptions
    public static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    //Claim Keys
    public static readonly string NameClaimKey = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
    public static readonly string EmailAddressClaimKey = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
    public static readonly string RoleClaimKey = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
    public static readonly string ExpiryDateClaimKey = "exp";

    //AdminKey
    public static readonly string AdminKey = "9bad5eb4-86fd-4d69-a28f-cb353bbc4d48-64f29fe9-d198-4e7d-8784-e396dc9a4756";
}
