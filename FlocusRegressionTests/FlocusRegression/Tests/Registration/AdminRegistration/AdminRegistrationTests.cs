using FlocusRegressionTests.Common;
using FlocusRegressionTests.Common.HelperMethods;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Net;
using Xunit;
using Xunit.Extensions.Ordering;

namespace FlocusRegressionTests.Tests.Registration.AdminRegistration;

[Collection("Admin Registration"), Order(2)]
public sealed class AdminRegistrationTests
{
    private readonly AdminRegistrationTestFixture _fixture;

    public AdminRegistrationTests(AdminRegistrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact, Order(1)]
    public async Task RegisterAdmin_ValidInputs_ShouldReturn200()
    {
        //Arrange
        var requestBody = new Dictionary<string, object>
            {
                { Constants.UsernameRequestKey, _fixture.Username },
                { Constants.PasswordRequestKey, _fixture.Password },
                { Constants.EmailAddressRequestKey, _fixture.EmailAddress },
                { Constants.IsAdminRequestKey, _fixture.IsAdmin },
                { Constants.KeyRequestKey, Constants.AdminKey }
            };

        //Act
        var response = await _fixture.HttpClient.PostAsync(Constants.RegisterSegment, TestHelpers.GetStringContentFromDict(requestBody));

        //Assert
        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            TestHelpers.GetHttpResponseBodyAsString(response).Should().Be("");
        }
    }
}
