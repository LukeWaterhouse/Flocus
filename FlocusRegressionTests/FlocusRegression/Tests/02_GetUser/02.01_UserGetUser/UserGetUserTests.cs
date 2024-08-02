using FlocusRegressionTests.Common;
using FlocusRegressionTests.Common.HelperMethods;
using FlocusRegressionTests.Common.Models.ErrorResponse;
using FlocusRegressionTests.Common.Models.UserResponse;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Net;
using Xunit;
using Xunit.Extensions.Ordering;

namespace FlocusRegressionTests.Tests._02_GetUser._02._01_UserGetUser;

[Collection("User GetUser"), Order(3)]
public sealed class UserGetUserTests
{
    private readonly UserGetUserTestFixture _fixture;

    public UserGetUserTests(UserGetUserTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact, Order(1)]
    public async Task GetUser_AsUserNoBearerToken_Returns401()
    {
        // Arrange
        TestHelpers.SetAccessToken(_fixture.HttpClient, null);

        // Act
        var response = await _fixture.HttpClient.GetAsync(Constants.GetUserSegment);

        // Assert
        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            TestHelpers.GetHttpResponseBodyAsString(response).Should().Be("");
        }
    }

    [Fact, Order(2)]
    public async Task GetDifferentUser_AsUserBearerToken_Returns403()
    {
        // Arrange
        TestHelpers.SetAccessToken(_fixture.HttpClient, _fixture.DifferentAccessToken);

        // Act
        var response = await _fixture.HttpClient.GetAsync(string.Format(Constants.GetUserAsAdminSegmentTemplate, _fixture.Username));

        // Assert
        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            TestHelpers.GetHttpResponseBodyAsString(response).Should().Be("");
        }
    }

    [Fact, Order(3)]
    public async Task GetUser_AsUserBearerToken_Returns200()
    {
        // Arrange
        TestHelpers.SetAccessToken(_fixture.HttpClient, _fixture.AccessToken);

        // Act
        var response = await _fixture.HttpClient.GetAsync(Constants.GetUserSegment);

        // Assert
        var responseUser = TestHelpers.DeserializeHttpResponseBody<UserDto>(response);
        var expectedUser = new UserDto(_fixture.Username, _fixture.EmailAddress, DateTime.UtcNow);

        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseUser.Username.Should().Be(expectedUser.Username);
            responseUser.EmailAddress.Should().Be(expectedUser.EmailAddress);
            responseUser.CreatedAt.Should().BeCloseTo(expectedUser.CreatedAt, TimeSpan.FromMinutes(2));
        }
    }
}
