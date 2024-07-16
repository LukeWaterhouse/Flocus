using FlocusRegressionTests.Common;
using FlocusRegressionTests.Common.HelperMethods;
using FlocusRegressionTests.Common.Models.ErrorResponse;
using FlocusRegressionTests.Common.Models.UserResponse;
using FluentAssertions;
using FluentAssertions.Execution;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using Xunit;
using Xunit.Extensions.Ordering;

namespace FlocusRegressionTests.Tests.Registration.UserRegistration;

[Collection("User Registration"), Order(1)]
public sealed class UserRegistrationTests
{
    private readonly UserRegistrationTestFixture _fixture;

    public UserRegistrationTests(UserRegistrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact, Order(1)]
    public async Task RegisterUser_ValidInputs_ShouldReturn200()
    {
        //Arrange
        var requestBody = new Dictionary<string, object>
            {
                { Constants.UsernameRequestKey, _fixture.Username },
                { Constants.PasswordRequestKey, _fixture.Password },
                { Constants.EmailAddressRequestKey, _fixture.EmailAddress },
                { Constants.IsAdminRequestKey, _fixture.IsAdmin },
                { Constants.KeyRequestKey, _fixture.Key }
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

    [Fact, Order(2)]
    public async Task RegisterUser_ExistingUsername_ShouldReturn409()
    {
        //Arrange
        var requestBody = new Dictionary<string, object>
            {
                { Constants.UsernameRequestKey, _fixture.Username },
                { Constants.PasswordRequestKey, _fixture.Password },
                { Constants.EmailAddressRequestKey, _fixture.EmailAddress },
                { Constants.IsAdminRequestKey, _fixture.IsAdmin },
                { Constants.KeyRequestKey, _fixture.Key }
            };

        //Act
        var response = await _fixture.HttpClient.PostAsync(Constants.RegisterSegment, TestHelpers.GetStringContentFromDict(requestBody));

        //Assert
        var errors = TestHelpers.DeserializeHttpResponseBody<ErrorsDto>(response);

        var expectedErrors = new ErrorsDto(
            new List<ErrorDto>
            {
                new ErrorDto(409, "user already exists with username: lukosparta123")
            });

        using (new AssertionScope())
        {
            errors.Should().BeEquivalentTo(expectedErrors);
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }
    }

    [Fact, Order(3)]
    public async Task RegisterUser_ExistingEmail_ShouldReturn409()
    {
        //Arrange
        var requestBody = new Dictionary<string, object>
            {
                { Constants.UsernameRequestKey, "different username" },
                { Constants.PasswordRequestKey, _fixture.Password },
                { Constants.EmailAddressRequestKey, _fixture.EmailAddress },
                { Constants.IsAdminRequestKey, _fixture.IsAdmin },
                { Constants.KeyRequestKey, _fixture.Key }
            };

        //Act
        var response = await _fixture.HttpClient.PostAsync(Constants.RegisterSegment, TestHelpers.GetStringContentFromDict(requestBody));

        //Assert
        var errors = TestHelpers.DeserializeHttpResponseBody<ErrorsDto>(response);

        var expectedErrors = new ErrorsDto(
            new List<ErrorDto>
            {
                new ErrorDto(409, $"user already exists with email: {_fixture.EmailAddress}")
            });

        using (new AssertionScope())
        {
            errors.Should().BeEquivalentTo(expectedErrors);
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }
    }

    [Fact, Order(4)]
    public async Task GetToken_IncorrectPassword_ShouldReturn401()
    {
        //Arrange
        var requestBody = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { Constants.UsernameRequestKey, _fixture.Username },
                { Constants.PasswordRequestKey, "wrong password" }
            });

        //Act
        var response = await _fixture.HttpClient.PostAsync(Constants.GetTokenSegment, requestBody);

        //Assert
        var errors = TestHelpers.DeserializeHttpResponseBody<ErrorsDto>(response);

        var expectedErrors = new ErrorsDto(
            new List<ErrorDto>
            {
                new ErrorDto(401, "Invalid username and password combination")
            });

        using (new AssertionScope())
        {
            errors.Should().BeEquivalentTo(expectedErrors);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }

    [Fact, Order(5)]
    public async Task GetToken_CorrectDetails_ShouldReturn200()
    {
        //Arrange
        var requestBody = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { Constants.UsernameRequestKey, _fixture.Username },
                { Constants.PasswordRequestKey, _fixture.Password }
            });

        //Act
        var response = await _fixture.HttpClient.PostAsync(Constants.GetTokenSegment, requestBody);

        //Assert
        var token = TestHelpers.GetHttpResponseBodyAsString(response);
        _fixture.AccessToken = token;

        var jwtHandler = new JwtSecurityTokenHandler();
        var jwt = jwtHandler.ReadJwtToken(token);
        var claims = jwt.Claims.ToList();

        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            token.Should().NotBeNull();
            claims[0].Type.Should().Be("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
            claims[0].Value.Should().Be(_fixture.Username);

            claims[1].Type.Should().Be("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
            claims[1].Value.Should().Be(_fixture.EmailAddress);

            claims[2].Type.Should().Be("http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
            claims[2].Value.Should().Be("User");

            claims[3].Type.Should().Be("exp");
            TestHelpers.UnixTimeStampStringToDateTime(claims[3].Value).Should().BeCloseTo(DateTime.UtcNow.AddDays(1), TimeSpan.FromMinutes(1));
        }
    }

    [Fact, Order(6)]
    public async Task GetUser_Unauthenticated_Returns401()
    {
        //Arrange
        SetAccessTokenAsync(true);

        //Act
        var response = await _fixture.HttpClient.GetAsync(Constants.GetUserSegment);

        //Assert
        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            TestHelpers.GetHttpResponseBodyAsString(response).Should().Be("");
        }
    }

    [Fact, Order(7)]
    public async Task GetUser_Authenticated_Returns200()
    {
        //Arrange
        SetAccessTokenAsync();

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

    [Fact, Order(8)]
    public async Task DeleteUser_Unauthenticated_Returns401()
    {
        //Arrange
        SetAccessTokenAsync(true);

        var requestBody = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { Constants.UsernameRequestKey, _fixture.Username },
                { Constants.PasswordRequestKey, _fixture.Password }
            });

        //Act
        var response = await _fixture.HttpClient.SendAsync(
            new HttpRequestMessage(HttpMethod.Delete, Constants.DeleteUserAsUserSegment)
            {
                Content = requestBody
            });

        //Assert
        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            TestHelpers.GetHttpResponseBodyAsString(response).Should().Be("");
        }
    }

    [Fact, Order(9)]
    public async Task DeleteUser_WrongPassword_Returns401()
    {
        //Arrange
        SetAccessTokenAsync();

        var requestBody = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { Constants.UsernameRequestKey, _fixture.Username },
                { Constants.PasswordRequestKey, "wrong password" }
            });

        //Act
        var response = await _fixture.HttpClient.SendAsync(
            new HttpRequestMessage(HttpMethod.Delete, Constants.DeleteUserAsUserSegment)
            {
                Content = requestBody
            });

        //Assert
        var errors = TestHelpers.DeserializeHttpResponseBody<ErrorsDto>(response);

        var expectedErrors = new ErrorsDto(
            new List<ErrorDto>
            {
                new ErrorDto(401, "Invalid username and password combination")
            });

        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            errors.Should().BeEquivalentTo(expectedErrors);
        }
    }

    [Fact, Order(10)]
    public async Task DeleteUser_UsernameMismatch_Returns403()
    {
        //Arrange
        var differentUsername = "different username";
        var requestBody = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { Constants.UsernameRequestKey, differentUsername },
                { Constants.PasswordRequestKey, _fixture.Password }
            });

        //Act
        var response = await _fixture.HttpClient.SendAsync(
            new HttpRequestMessage(HttpMethod.Delete, Constants.DeleteUserAsUserSegment)
            {
                Content = requestBody
            });

        //Assert
        var errors = TestHelpers.DeserializeHttpResponseBody<ErrorsDto>(response);

        var expectedErrors = new ErrorsDto(
            new List<ErrorDto>
            {
                new ErrorDto(403, $"Not authorized to delete user: '{differentUsername}'")
            });

        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            errors.Should().BeEquivalentTo(expectedErrors);
        }
    }

    [Fact, Order(11)]
    public async Task DeleteUser_ValidCredentials_Returns200()
    {
        //Arrange
        var requestBody = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { Constants.UsernameRequestKey, _fixture.Username },
                { Constants.PasswordRequestKey, _fixture.Password }
            });

        //Act
        var response = await _fixture.HttpClient.SendAsync(
            new HttpRequestMessage(HttpMethod.Delete, Constants.DeleteUserAsUserSegment)
            {
                Content = requestBody
            });

        //Assert
        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            TestHelpers.GetHttpResponseBodyAsString(response).Should().Be("");
        }
    }

    [Fact, Order(12)]
    public async Task GetUser_DeletedUser_Returns404()
    {
        //Act
        var response = await _fixture.HttpClient.GetAsync(Constants.GetUserSegment);

        //Assert
        var errors = TestHelpers.DeserializeHttpResponseBody<ErrorsDto>(response);

        var expectedErrors = new ErrorsDto(
            new List<ErrorDto>
            {
                new ErrorDto(404, "No user could be found with username: " + _fixture.Username)
            });

        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            errors.Should().BeEquivalentTo(expectedErrors);
        }
    }

    #region HelperMethods   

    private void SetAccessTokenAsync(bool setAsNull = false)
    {
        if (!setAsNull)
        {
            _fixture.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _fixture.AccessToken);
            return;
        }
        _fixture.HttpClient.DefaultRequestHeaders.Authorization = null;
    }
    #endregion
}
