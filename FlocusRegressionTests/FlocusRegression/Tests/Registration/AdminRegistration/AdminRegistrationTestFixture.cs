using FlocusRegressionTests.Common;
using FlocusRegressionTests.Common.HelperMethods;
using System.Text.Json;

namespace FlocusRegressionTests.Tests.Registration.AdminRegistration;

public sealed class AdminRegistrationTestFixture : IDisposable
{
    public HttpClient HttpClient { get; set; }
    public JsonSerializerOptions JsonSerializerOptions { get; set; }
    public string AccessToken { get; set; }

    public readonly string Username = "MasterChief";
    public readonly string Password = "Kratos1234";
    public readonly string EmailAddress = "MasterChief@hotmail.co.uk";
    public readonly bool IsAdmin = true;

    public AdminRegistrationTestFixture()
    {
        HttpClient = new HttpClient
        {
            BaseAddress = new Uri(Constants.BaseUrl),
        };

        TestHelpers.EnsureNoExsitingUser(HttpClient, Username, Password).Wait();
    }

    public void Dispose()
    {
        TestHelpers.EnsureNoExsitingUser(HttpClient, Username, Password).Wait();
        return;
    }
}
