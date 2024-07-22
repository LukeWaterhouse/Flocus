using FlocusRegressionTests.Common;
using FlocusRegressionTests.Common.HelperMethods;
using FlocusRegressionTests.Common.Models.ErrorResponse;
using FlocusRegressionTests.Common.Models.UserResponse;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Net;
using Xunit;
using Xunit.Extensions.Ordering;

namespace FlocusRegressionTests.Tests._02_GetUser._02._02_AdminGetUser;

[Collection("Admin GetUser"), Order(4)]
public sealed class AdminGetUserTests
{
    private readonly AdminGetUserTestFixture _fixture;

    public AdminGetUserTests(AdminGetUserTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact, Order(1)]
    public async Task GetAdminUser_AsAdminUserBearerToken_Returns200()
    {
        //Arrange
        TestHelpers.SetAccessToken(_fixture.HttpClient, _fixture.AccessToken);

        //Act
        var response = await _fixture.HttpClient.GetAsync(Constants.GetUserSegment);

        //Assert
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

    [Fact, Order(2)]
    public async Task GetDifferentUser_AsAdminBearerToken_Returns200()
    {
        //Arrange
        TestHelpers.SetAccessToken(_fixture.HttpClient, _fixture.AccessToken);

        //Act
        var response = await _fixture.HttpClient.GetAsync(Constants.GetUserSegment + $"?username={_fixture.DifferentUsername}");

        //Assert
        var responseUser = TestHelpers.DeserializeHttpResponseBody<UserDto>(response);
        var expectedUser = new UserDto(_fixture.DifferentUsername, _fixture.DifferentEmailAddress, DateTime.UtcNow);

        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseUser.Username.Should().Be(expectedUser.Username);
            responseUser.EmailAddress.Should().Be(expectedUser.EmailAddress);
            responseUser.CreatedAt.Should().BeCloseTo(expectedUser.CreatedAt, TimeSpan.FromMinutes(2));
        }
    }

    [Fact, Order(3)]
    public async Task GetDifferentUser_AsAdminBearerTokenUserNotValid_Returns404()
    {
        //Arrange
        var nonExistingUsername = "invalidUsername";

        TestHelpers.SetAccessToken(_fixture.HttpClient, _fixture.AccessToken);

        //Act
        var response = await _fixture.HttpClient.GetAsync(Constants.GetUserSegment + $"?username={nonExistingUsername}");

        //Assert
        var errors = TestHelpers.DeserializeHttpResponseBody<ErrorsDto>(response);
        var expectedErrors = new ErrorsDto(
            new List<ErrorDto>
            {
                new ErrorDto(404, $"No user could be found with username: {nonExistingUsername}")
            });

        using (new AssertionScope())
        {
            errors.Should().BeEquivalentTo(expectedErrors);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
