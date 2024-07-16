using System.Text.Json;

namespace FlocusRegressionTests.Tests.Registration.AdminRegistration;

public sealed class AdminRegistrationTestFixture
{
    public HttpClient HttpClient { get; set; }
    public JsonSerializerOptions JsonSerializerOptions { get; set; }
    public string AccessToken { get; set; }

}
