using System.Net;
using System.Net.Http.Headers;

namespace FlocusRegressionTests.Common.HelperMethods;

public static class TestHelpers
{

    public static async Task EnsureNoExsitingUser(HttpClient httpClient, string username, string password)
    {
        var requestBody = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { Constants.UsernameRequestKey, username },
                { Constants.PasswordRequestKey, password }
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
}
