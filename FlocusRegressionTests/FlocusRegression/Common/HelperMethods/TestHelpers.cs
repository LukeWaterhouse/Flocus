using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FlocusRegressionTests.Common.HelperMethods;

public static class TestHelpers
{
    public static async Task EnsureNoExsitingUser(HttpClient httpClient, string username, string password)
    {
        var requestBody = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { Constants.UsernameRequestKey, username },
                { Constants.PasswordRequestKey, password }//do this as super user so always works
            });

        var getTokenResponse = await httpClient.PostAsync(Constants.GetTokenSegment, requestBody);

        var token = await getTokenResponse.Content.ReadAsStringAsync();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var deleteResponse = await httpClient.SendAsync(
            new HttpRequestMessage(HttpMethod.Delete, Constants.DeleteUserAsUserSegment)
            {
                Content = requestBody
            });

        if (getTokenResponse.StatusCode != HttpStatusCode.Unauthorized && getTokenResponse.StatusCode != HttpStatusCode.OK)
        {
            throw new Exception($"getToken response during preparation should have status code {HttpStatusCode.Unauthorized} or {HttpStatusCode.OK}");
        }

        if (getTokenResponse.StatusCode == HttpStatusCode.OK && deleteResponse.StatusCode != HttpStatusCode.OK)
        {
            throw new Exception("preparationUser was not successfully reset");
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
