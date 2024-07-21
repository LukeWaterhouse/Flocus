using FlocusRegressionTests.Common;
using FlocusRegressionTests.Common.HelperMethods;
using System.Text.Json;

namespace FlocusRegressionTests.Tests.Registration.AdminRegistration;

public sealed class AdminRegistrationAndDeletionTestFixture : IDisposable
{
    public HttpClient HttpClient { get; set; }
    public JsonSerializerOptions JsonSerializerOptions { get; set; }
    public string AccessToken { get; set; }

    public readonly string Username = "MasterChief";
    public readonly string Password = "Kratos!234";
    public readonly string EmailAddress = "MasterChief@hotmail.co.uk";
    public readonly bool IsAdmin = true;

    public readonly string DifferentAdminUsername = "differentAdminName";
    public readonly string DifferentUserUsername = "differentUserName";

    public AdminRegistrationAndDeletionTestFixture()
    {
        HttpClient = new HttpClient
        {
            BaseAddress = new Uri(Constants.BaseUrl)
        };

        Cleanup().Wait();

        TestHelpers.CreateUser(DifferentAdminUsername, "differentAdminPassword!234", "differentAdminEmail@hotmail.com", true).Wait();
        TestHelpers.CreateUser(DifferentUserUsername, "differentUserPassword!234", "differentUserEmail@hotmail.com", false).Wait();
    }

    public void Dispose()
    {
        Cleanup().Wait();
        return;
    }

    private async Task Cleanup()
    {
        await TestHelpers.EnsureNoExsitingAccount(Username, IsAdmin);
        await TestHelpers.EnsureNoExsitingAccount(DifferentAdminUsername, true);
        await TestHelpers.EnsureNoExsitingAccount(DifferentUserUsername, false);
    }
}
