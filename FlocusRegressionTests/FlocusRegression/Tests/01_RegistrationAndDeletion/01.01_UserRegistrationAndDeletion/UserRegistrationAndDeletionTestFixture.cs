using FlocusRegressionTests.Common;
using FlocusRegressionTests.Common.HelperMethods;

namespace FlocusRegressionTests.Tests.Registration.UserRegistration;

public sealed class UserRegistrationAndDeletionTestFixture : IDisposable
{
    public HttpClient HttpClient { get; set; }
    public string AccessToken { get; set; }

    public readonly string Username = "lukosparta123";
    public readonly string Password = "Rollo!234";
    public readonly string EmailAddress = "lukewwaterhouse@hotmail.co.uk";
    public readonly bool IsAdmin = false;
    public readonly string Key = "n/a";

    public readonly string DifferentUserUsername = "differentUsername";

    public UserRegistrationAndDeletionTestFixture()
    {
        HttpClient = new HttpClient
        {
            BaseAddress = new Uri(Constants.BaseUrl),
        };
        Cleanup().Wait();
    }

    public void Dispose()
    {
        Cleanup().Wait();
        return;
    }

    private async Task Cleanup()
    {
        await TestHelpers.EnsureNoExsitingAccount(Username, IsAdmin);
        await TestHelpers.EnsureNoExsitingAccount(DifferentUserUsername, false);
    }
}

