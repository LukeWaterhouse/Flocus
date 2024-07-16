using FlocusRegressionTests.Common;
using FlocusRegressionTests.Common.HelperMethods;

namespace FlocusRegressionTests.Tests.Registration.UserRegistration;

public sealed class UserRegistrationTestFixture : IDisposable
{
    public HttpClient HttpClient { get; set; }
    public string AccessToken { get; set; }

    public readonly string Username = "lukosparta123";
    public readonly string Password = "rollo123";
    public readonly string EmailAddress = "lukewwaterhouse@hotmail.co.uk";
    public readonly bool IsAdmin = false;
    public readonly string Key = "n/a";

    public UserRegistrationTestFixture()
    {
        HttpClient = new HttpClient
        {
            BaseAddress = new Uri(Constants.BaseUrl),
        };

        TestHelpers.EnsureNoExsitingUser(HttpClient, Username, Password, IsAdmin).Wait();
    }

    public void Dispose()
    {
        TestHelpers.EnsureNoExsitingUser(HttpClient, Username, Password, IsAdmin).Wait();
        return;
    }
}

