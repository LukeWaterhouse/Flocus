using FlocusRegressionTests.Common.Models.ErrorResponse;
using FlocusRegressionTests.Common.Models.UserResponse;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FlocusRegressionTests.Common.HelperMethods;

//TODO: see about separating out into multiple test helper classes, maybe one for api calls and another for synchronous methods
public static class TestHelpers
{
    private readonly static string testHelperUserUsername = "testHelperUser";
    private readonly static string testHelperUserPassword = "testHelperUserPassword!234";
    private readonly static string testHelperUserEmail = "testHelperUserEmail@hotmail.com";

    private readonly static HttpClient HttpClient = new HttpClient
    {
        BaseAddress = new Uri(Constants.BaseUrl)
    };

    public static async Task EnsureNoExistingAccount(string username, bool isAdmin)
    {
        await EnsureTestHelperUserAndSetToken();
        HttpStatusCode? deleteStatusCode = null;

        if (!isAdmin)
        {
            var deleteResponse = await HttpClient.SendAsync(
                new HttpRequestMessage(HttpMethod.Delete, string.Format(Constants.DeleteByNameSegmentTemplate, username)));
            deleteStatusCode = deleteResponse.StatusCode;
        }

        if (isAdmin)
        {
            var deleteAdminAsAdminRequestBody = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { Constants.AdminKeyRequestKey, Constants.AdminKey }
            });

            var deleteResponse = await HttpClient.SendAsync(
                new HttpRequestMessage(HttpMethod.Delete, string.Format(Constants.DeleteByNameSegmentTemplate, username))
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

    public static async Task<(HttpStatusCode statusCode, UserDto? User, ErrorsDto? Errors)> TryGetUser(string username)
    {
        await EnsureTestHelperUserAndSetToken();

        var response = await HttpClient.GetAsync(string.Format(Constants.GetUserSegment + $"?username={username}"));

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            var errors = DeserializeHttpResponseBody<ErrorsDto>(response);
            return (response.StatusCode, null, errors);
        }

        if (response.StatusCode == HttpStatusCode.OK)
        {
            return (response.StatusCode, DeserializeHttpResponseBody<UserDto>(response), null);
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
        return JsonSerializer.Deserialize<T>(body, Constants.JsonSerializerOptions)
            ?? throw new Exception($"Failed to deserialize string into {typeof(T).Name}: {body}");
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

    public static void SetAccessToken(HttpClient httpClient, string? accessToken)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    public static async Task<string> GetAccessToken(string username, string password)
    {
        var requestBody = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { Constants.UsernameRequestKey, username },
                { Constants.PasswordRequestKey, password }
            });
        var response = await HttpClient.PostAsync(Constants.GetTokenSegment, requestBody);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new Exception($"Error retrieving access token for user: '{username}'. StatusCode: {response.StatusCode}");
        }
        return GetHttpResponseBodyAsString(response);
    }

    #region Private Methods
    private static async Task EnsureTestHelperUserAndSetToken()
    {
        //Create admin user
        var adminUserRegisterRequestBody = new Dictionary<string, object>
            {
                { Constants.UsernameRequestKey, testHelperUserUsername },
                { Constants.PasswordRequestKey, testHelperUserPassword },
                { Constants.EmailAddressRequestKey, testHelperUserEmail },
                { Constants.IsAdminRequestKey, true },
                { Constants.AdminKeyRequestKey, Constants.AdminKey }
            };
        var response = await HttpClient.PostAsync(Constants.RegisterSegment, GetStringContentFromDict(adminUserRegisterRequestBody));
        if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Conflict)
        {
            var errors = DeserializeHttpResponseBody<ErrorsDto>(response);
            throw new Exception($"failed to create admin user for test preparation: {errors}");
        }

        var adminToken = await GetAccessToken(testHelperUserUsername, testHelperUserPassword);

        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
    }
    #endregion
}
