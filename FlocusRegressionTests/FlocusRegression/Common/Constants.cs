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
    public static readonly string GetTokenSegment = IdentityController + "/getToken";
    public static readonly string RegisterSegment = IdentityController + "/register";
    public static readonly string DeleteUserAsUserSegment = IdentityController + "/deleteUserAsUser";

    public static readonly string GetUserSegment = UserController + "/getUser";

    //Request Form Keys
    public static readonly string UsernameRequestKey = "username";
    public static readonly string PasswordRequestKey = "password";
    public static readonly string EmailAddressRequestKey = "emailAddress";
    public static readonly string IsAdminRequestKey = "isAdmin";
    public static readonly string KeyRequestKey = "key";

    //Test Collections
    public static readonly string UserRegistration = "UserRegistration";

    //JsonSerializerOptions
    public static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    //AdminKey
    public static readonly string AdminKey = "9bad5eb4-86fd-4d69-a28f-cb353bbc4d48-64f29fe9-d198-4e7d-8784-e396dc9a4756";
}
