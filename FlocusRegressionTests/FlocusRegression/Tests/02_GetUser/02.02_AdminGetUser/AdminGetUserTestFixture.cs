using FlocusRegressionTests.Common;
using FlocusRegressionTests.Common.HelperMethods;

namespace FlocusRegressionTests.Tests._02_GetUser._02._02_AdminGetUser;

public sealed class AdminGetUserTestFixture : IDisposable
{
    public HttpClient HttpClient { get; set; }
    public string AccessToken { get; set; } = "";
    public string DifferentAccessToken { get; set; } = "";

    public readonly string Username = "adminGetUser";
    public readonly string Password = "Rollo!234";
    public readonly string EmailAddress = "lukewwaterhouse@hotmail.co.uk";
    public readonly bool IsAdmin = true;
    public readonly string Key = "n/a";

    public readonly string DifferentUsername = "randomUser";
    public readonly string DifferentPassword = "differentPassword!234";
    public readonly string DifferentEmailAddress = "differentEmail@hotmail.com";
    public readonly bool DifferentIsAdmin = false;

    public AdminGetUserTestFixture()
    {
        HttpClient = new HttpClient
        {
            BaseAddress = new Uri(Constants.BaseUrl),
        };

        Cleanup().Wait();
        Prepare().Wait();
    }

    public void Dispose()
    {
        Cleanup().Wait();
    }

    private async Task Prepare()
    {
        await TestHelpers.CreateUser(Username, Password, EmailAddress, IsAdmin);
        await TestHelpers.CreateUser(DifferentUsername, DifferentPassword, DifferentEmailAddress, DifferentIsAdmin);

        var accessToken = await TestHelpers.GetAccessToken(Username, Password);
        AccessToken = accessToken;

        var differentAccessToken = await TestHelpers.GetAccessToken(DifferentUsername, DifferentPassword);
        DifferentAccessToken = differentAccessToken;
    }

    private async Task Cleanup()
    {
        await TestHelpers.EnsureNoExistingAccount(Username, IsAdmin);
        await TestHelpers.EnsureNoExistingAccount(DifferentUsername, DifferentIsAdmin);
    }
}
