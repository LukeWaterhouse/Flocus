using FlocusRegressionTests.Common.Models.UserResponse;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FlocusRegressionTests.Common.HelperMethods;

public static class TestHelpers
{

    private readonly static string superUserUsername = "adminUser";
    private readonly static string superUserPassword = "adminUserPassword";
    private readonly static string superUserEmail = "adminUserEmail@hotmail.com";

    private readonly static HttpClient HttpClient = new HttpClient
    {
        BaseAddress = new Uri(Constants.BaseUrl)
    };

    public static async Task EnsureNoExsitingAccount(string username, bool isAdmin)
    {
        var accessToken = await EnsureTestHelperUserAndGetToken();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        HttpStatusCode? deleteStatusCode = null;

        if (!isAdmin)
        {
            var deleteUserAsAdminRequestBody = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { Constants.UsernameRequestKey, username }
            });

            var deleteResponse = await HttpClient.SendAsync(
                new HttpRequestMessage(HttpMethod.Delete, Constants.DeleteUserAsAdmin)
                {
                    Content = deleteUserAsAdminRequestBody
                });
            deleteStatusCode = deleteResponse.StatusCode;
        }

        if (isAdmin)
        {
            var deleteAdminAsAdminRequestBody = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { Constants.UsernameRequestKey, username },
                { Constants.AdminKeyRequestKey, Constants.AdminKey }
            });

            var deleteResponse = await HttpClient.SendAsync(
                new HttpRequestMessage(HttpMethod.Delete, Constants.DeleteAdmin)
                {
                    Content = deleteAdminAsAdminRequestBody
                });
            deleteStatusCode = deleteResponse.StatusCode;
        }

        if (deleteStatusCode != HttpStatusCode.OK && deleteStatusCode != HttpStatusCode.NotFound)
        {
            throw new Exception($"deleteAccount response during preparation should have status code {HttpStatusCode.NotFound} or {HttpStatusCode.OK}");
        }
    }

    public static async Task CreateUser(string username, string password, string email, bool isAdmin)
    {
        var registerRequestBody = new Dictionary<string, object>
            {
                { Constants.UsernameRequestKey, username },
                { Constants.PasswordRequestKey, password },
                { Constants.EmailAddressRequestKey, email },
                { Constants.IsAdminRequestKey, isAdmin },
            };

        if (isAdmin)
        {
            registerRequestBody.Add(Constants.AdminKeyRequestKey, Constants.AdminKey);
        }

        var response = await HttpClient.PostAsync(Constants.RegisterSegment, GetStringContentFromDict(registerRequestBody));
        if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Conflict)
        {
            throw new Exception($"failed to create user for test preparation: {username}");
        }
    }

    public static async Task<(UserDto? User, HttpStatusCode statusCode)> GetUser(string username)
    {
        var accessToken = await EnsureTestHelperUserAndGetToken();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await HttpClient.GetAsync(Constants.GetUserSegment + $"?username={username}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return (null, response.StatusCode);
        }

        if (response.StatusCode == HttpStatusCode.OK)
        {
            return (DeserializeHttpResponseBody<UserDto>(response), response.StatusCode);
        }
        throw new Exception("Get user status code was not 200 or 404");
    }

    public static StringContent GetStringContentFromDict(Dictionary<string, object> dictionary)
    {
        var json = JsonSerializer.Serialize(dictionary);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    public static T DeserializeHttpResponseBody<T>(HttpResponseMessage httpResponseMessage)
    {
        var body = httpResponseMessage.Content.ReadAsStringAsync().Result;
        return JsonSerializer.Deserialize<T>(body, Constants.JsonSerializerOptions) ?? throw new Exception($"Failed to deserialize string into {typeof(T).Name}: {body}");
    }

    public static string GetHttpResponseBodyAsString(HttpResponseMessage httpResponseMessage)
    {
        return httpResponseMessage.Content.ReadAsStringAsync().Result;
    }

    public static DateTime UnixTimeStampStringToDateTime(string unixTimestampString)
    {
        if (!long.TryParse(unixTimestampString, out long unixTimestamp))
        {
            throw new ArgumentException("Invalid Unix timestamp string", nameof(unixTimestampString));
        }

        DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return unixEpoch.AddSeconds(unixTimestamp);
    }


    #region Helper Methods
    private static async Task<string> EnsureTestHelperUserAndGetToken()
    {
        //Create admin user
        var adminUserRegisterRequestBody = new Dictionary<string, object>
            {
                { Constants.UsernameRequestKey, superUserUsername },
                { Constants.PasswordRequestKey, superUserPassword },
                { Constants.EmailAddressRequestKey, superUserEmail },
                { Constants.IsAdminRequestKey, true },
                { Constants.AdminKeyRequestKey, Constants.AdminKey }
            };
        var response = await HttpClient.PostAsync(Constants.RegisterSegment, GetStringContentFromDict(adminUserRegisterRequestBody));
        if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Conflict)
        {
            throw new Exception("failed to create admin user for test preparation");
        }

        var adminUserGetTokenRequestBody = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { Constants.UsernameRequestKey, superUserUsername },
                { Constants.PasswordRequestKey, superUserPassword }
            });

        var getAdminTokenResponse = await HttpClient.PostAsync(Constants.GetTokenSegment, adminUserGetTokenRequestBody);
        var adminToken = await getAdminTokenResponse.Content.ReadAsStringAsync();
        return adminToken;
    }
    #endregion
}
