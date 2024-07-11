using FlocusRegressionTests.Common;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace FlocusRegressionTests.RegistrationTests;

[Collection("UserRegistrationTests")]
public sealed class UserRegistrationTests : IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly string GetTokenSegment = Constants.IdenitySegment + "/getToken";
    private readonly string RegisterSegment = Constants.IdenitySegment + "/register";
    private readonly string DeleteUserAsUserSegment = Constants.IdenitySegment + "/deleteUserAsUser";

    private string Username = "lukosparta123";
    private string Password = "rollo123";
    private string EmailAddress = "lukewwaterhouse@hotmail.co.uk";
    private bool IsAdmin = false;
    private string Key = "n/a";

    private string UsernameRequestKey = "username";
    private string PasswordRequestKey = "password";
    private string EmailAddressRequestKey = "emailAddress";
    private string IsAdminRequestKey = "isAdmin";
    private string KeyRequestKey = "key";

    public UserRegistrationTests()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(Constants.BaseUrl),
        };
    }

    [Fact]
    public async Task RegisterUser_ValidInputs_ShouldReturn200()
    {
        //Arrange
        var requestBody = new Dictionary<string, object>
            {
                { UsernameRequestKey, Username },
                { PasswordRequestKey, Password },
                { EmailAddressRequestKey, EmailAddress },
                { IsAdminRequestKey, IsAdmin },
                { KeyRequestKey, Key }
            };

        //Act
        var response = await _httpClient.PostAsync(RegisterSegment, GetContentFromDict(requestBody));

        //Assert
        var body = await response.Content.ReadAsStringAsync();

        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            body.Should().Be("");
        }
    }

    [Fact]
    public async Task RegisterUser_ExistingUsername_ShouldReturn409()
    {
        //Arrange
        var requestBody = new Dictionary<string, object>
            {
                { UsernameRequestKey, Username },
                { PasswordRequestKey, Password },
                { EmailAddressRequestKey, EmailAddress },
                { IsAdminRequestKey, IsAdmin },
                { KeyRequestKey, Key }
            };

        //Act
        var response = await _httpClient.PostAsync(RegisterSegment, GetContentFromDict(requestBody));

        //Assert
        var body = await response.Content.ReadAsStringAsync();

        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            body.Should().Be("");
        }

    }

    #region HelperMethods
    public async Task InitializeAsync()
    {
        await EnsureNoExistingUser();
    }

    public Task DisposeAsync()
    {
        _httpClient.Dispose(); //might not need to do this
        return Task.CompletedTask;
    }

    private async Task EnsureNoExistingUser()
    {
        var getTokenRequestBody = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { UsernameRequestKey, Username },
                { PasswordRequestKey, Password }
            });

        var getTokenResponse = await _httpClient.PostAsync(GetTokenSegment, getTokenRequestBody);

        var token = await getTokenResponse.Content.ReadAsStringAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var deleteResponse = await _httpClient.SendAsync(
            new HttpRequestMessage(HttpMethod.Delete, DeleteUserAsUserSegment)
            {
                Content = getTokenRequestBody
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

    private StringContent GetContentFromDict(Dictionary<string, object> dictionary)
    {
        var json = JsonSerializer.Serialize(dictionary);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
    #endregion
}
