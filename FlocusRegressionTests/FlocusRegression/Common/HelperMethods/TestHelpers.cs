using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FlocusRegressionTests.Common.HelperMethods;

public static class TestHelpers
{
    public static async Task EnsureNoExsitingAccount(HttpClient httpClient, string username, bool isAdmin)
    {
        //Create admin user
        var adminUserUsername = "adminUser";
        var adminUserPassword = "adminUserPassword";
        var adminUserEmail = "adminUserEmail@hotmail.com";

        var adminUserRegisterRequestBody = new Dictionary<string, object>
            {
                { Constants.UsernameRequestKey, adminUserUsername },
                { Constants.PasswordRequestKey, adminUserPassword },
                { Constants.EmailAddressRequestKey, adminUserEmail },
                { Constants.IsAdminRequestKey, true },
                { Constants.AdminKeyRequestKey, Constants.AdminKey }
            };
        var response = await httpClient.PostAsync(Constants.RegisterSegment, GetStringContentFromDict(adminUserRegisterRequestBody));
        if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Conflict)
        {
            throw new Exception("failed to create admin user for test preparation");
        }

        var adminUserGetTokenRequestBody = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { Constants.UsernameRequestKey, adminUserUsername },
                { Constants.PasswordRequestKey, adminUserPassword }
            });

        var getAdminTokenResponse = await httpClient.PostAsync(Constants.GetTokenSegment, adminUserGetTokenRequestBody);
        var adminToken = await getAdminTokenResponse.Content.ReadAsStringAsync();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        HttpStatusCode? deleteStatusCode = null;

        //Delete passed in account
        if (!isAdmin)
        {
            var deleteUserAsAdminRequestBody = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { Constants.UsernameRequestKey, username }
            });

            var deleteResponse = await httpClient.SendAsync(
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

            var deleteResponse = await httpClient.SendAsync(
                new HttpRequestMessage(HttpMethod.Delete, Constants.DeleteAdmin)
                {
                    Content = deleteAdminAsAdminRequestBody
                });
            deleteStatusCode = deleteResponse.StatusCode;
        }

        httpClient.DefaultRequestHeaders.Authorization = null;

        if (deleteStatusCode != HttpStatusCode.OK && deleteStatusCode != HttpStatusCode.NotFound)
        {
            throw new Exception($"deleteAccount response during preparation should have status code {HttpStatusCode.NotFound} or {HttpStatusCode.OK}");
        }
    }

    public static async Task CreateUser(HttpClient httpClient, string username, string password, string email, bool isAdmin)
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

        var response = await httpClient.PostAsync(Constants.RegisterSegment, GetStringContentFromDict(registerRequestBody));
        if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Conflict)
        {
            throw new Exception($"failed to create user for test preparation: {username}");
        }
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
}
