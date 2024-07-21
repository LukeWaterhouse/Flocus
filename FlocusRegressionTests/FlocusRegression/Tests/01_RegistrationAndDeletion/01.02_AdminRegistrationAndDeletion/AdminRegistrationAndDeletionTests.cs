using FlocusRegressionTests.Common;
using FlocusRegressionTests.Common.HelperMethods;
using FlocusRegressionTests.Common.Models.ErrorResponse;
using FlocusRegressionTests.Common.Models.UserResponse;
using FluentAssertions;
using FluentAssertions.Execution;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Xunit;
using Xunit.Extensions.Ordering;

namespace FlocusRegressionTests.Tests.Registration.AdminRegistration;

[Collection("Admin Registration/Deletion"), Order(2)]
public sealed class AdminRegistrationAndDeletionTests
{
    private readonly AdminRegistrationAndDeletionTestFixture _fixture;

    public AdminRegistrationAndDeletionTests(AdminRegistrationAndDeletionTestFixture fixture)
    {
        _fixture = fixture;
    }

    #region Registration
    [Fact, Order(1)]
    public async Task RegisterAdmin_IncorrectAdminUserKey_ShouldReturn401()
    {
        //Arrange
        var requestBody = new Dictionary<string, object>
            {
                { Constants.UsernameRequestKey, _fixture.DifferentAdminUsername },
                { Constants.PasswordRequestKey, _fixture.Password },
                { Constants.EmailAddressRequestKey, _fixture.EmailAddress },
                { Constants.IsAdminRequestKey, _fixture.IsAdmin },
                { Constants.AdminKeyRequestKey, "incorrect key" }
            };

        //Act
        var response = await _fixture.HttpClient.PostAsync(Constants.RegisterSegment, TestHelpers.GetStringContentFromDict(requestBody));

        //Assert
        var errors = TestHelpers.DeserializeHttpResponseBody<ErrorsDto>(response);

        var expectedErrors = new ErrorsDto(
            new List<ErrorDto>
            {
                new ErrorDto(401, "Admin key is not correct")
            });

        using (new AssertionScope())
        {
            errors.Should().BeEquivalentTo(expectedErrors);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }

    [Fact, Order(2)]
    public async Task RegisterAdmin_NoAdminKey_ShouldReturn400()
    {
        //Arrange
        var requestBody = new Dictionary<string, object>
            {
                { Constants.UsernameRequestKey, _fixture.DifferentAdminUsername },
                { Constants.PasswordRequestKey, _fixture.Password },
                { Constants.EmailAddressRequestKey, _fixture.EmailAddress },
                { Constants.IsAdminRequestKey, _fixture.IsAdmin }
            };

        //Act
        var response = await _fixture.HttpClient.PostAsync(Constants.RegisterSegment, TestHelpers.GetStringContentFromDict(requestBody));

        //Assert
        var errors = TestHelpers.DeserializeHttpResponseBody<ErrorsDto>(response);

        var expectedErrors = new ErrorsDto(
            new List<ErrorDto>
            {
                new ErrorDto(400, "Must provide key when creating an admin")
            });

        using (new AssertionScope())
        {
            errors.Should().BeEquivalentTo(expectedErrors);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }

    [Fact, Order(3)]
    public async Task RegisterAdmin_ValidInputs_ShouldReturn200()
    {
        //Arrange
        var requestBody = new Dictionary<string, object>
            {
                { Constants.UsernameRequestKey, _fixture.Username },
                { Constants.PasswordRequestKey, _fixture.Password },
                { Constants.EmailAddressRequestKey, _fixture.EmailAddress },
                { Constants.IsAdminRequestKey, _fixture.IsAdmin },
                { Constants.AdminKeyRequestKey, Constants.AdminKey }
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

    [Fact, Order(4)]
    public async Task RegisterAdmin_ExistingUsername_ShouldReturn409()
    {
        //Arrange
        var requestBody = new Dictionary<string, object>
            {
                { Constants.UsernameRequestKey, _fixture.Username },
                { Constants.PasswordRequestKey, _fixture.Password },
                { Constants.EmailAddressRequestKey, _fixture.EmailAddress },
                { Constants.IsAdminRequestKey, _fixture.IsAdmin },
                { Constants.AdminKeyRequestKey, Constants.AdminKey }
            };

        //Act
        var response = await _fixture.HttpClient.PostAsync(Constants.RegisterSegment, TestHelpers.GetStringContentFromDict(requestBody));

        //Assert
        var errors = TestHelpers.DeserializeHttpResponseBody<ErrorsDto>(response);

        var expectedErrors = new ErrorsDto(
            new List<ErrorDto>
            {
                new ErrorDto(409, $"user already exists with username: {_fixture.Username}")
            });

        using (new AssertionScope())
        {
            errors.Should().BeEquivalentTo(expectedErrors);
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }
    }

    [Fact, Order(5)]
    public async Task RegisterAdmin_ExistingEmail_ShouldReturn409()
    {
        //Arrange
        var requestBody = new Dictionary<string, object>
            {
                { Constants.UsernameRequestKey, "nonExistingName" },
                { Constants.PasswordRequestKey, _fixture.Password },
                { Constants.EmailAddressRequestKey, _fixture.EmailAddress },
                { Constants.IsAdminRequestKey, _fixture.IsAdmin },
                { Constants.AdminKeyRequestKey, Constants.AdminKey }
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

    [Fact, Order(6)]
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

    [Fact, Order(7)]
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

        #region Set Access Token
        _fixture.AccessToken = token;
        #endregion

        var jwtHandler = new JwtSecurityTokenHandler();
        var jwt = jwtHandler.ReadJwtToken(token);
        var claims = jwt.Claims.ToList();

        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            token.Should().NotBeNull();
            claims[0].Type.Should().Be(Constants.NameClaimKey);
            claims[0].Value.Should().Be(_fixture.Username);

            claims[1].Type.Should().Be(Constants.EmailAddressClaimKey);
            claims[1].Value.Should().Be(_fixture.EmailAddress);

            claims[2].Type.Should().Be(Constants.RoleClaimKey);
            claims[2].Value.Should().Be("Admin");

            claims[3].Type.Should().Be(Constants.ExpiryDateClaimKey);
            TestHelpers.UnixTimeStampStringToDateTime(claims[3].Value).Should().BeCloseTo(DateTime.UtcNow.AddDays(1), TimeSpan.FromMinutes(1));
        }
    }

    [Fact, Order(8)]
    public async Task GetAdminUser_Unauthenticated_Returns401()
    {
        //Arrange
        TestHelpers.SetAccessToken(_fixture.HttpClient, null);

        //Act
        var response = await _fixture.HttpClient.GetAsync(Constants.GetUserSegment);

        //Assert
        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            TestHelpers.GetHttpResponseBodyAsString(response).Should().Be("");
        }
    }

    [Fact, Order(9)]
    public async Task GetAdminUser_Authenticated_Returns200()
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
    #endregion

    #region Deletion

    #region Deletion Unhappy Paths
    [Fact, Order(10)]
    public async Task DeleteSelfAdmin_Unauthenticated_Returns401()
    {
        //Arrange
        TestHelpers.SetAccessToken(_fixture.HttpClient, null);

        var requestBody = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { Constants.UsernameRequestKey, _fixture.Username },
                { Constants.PasswordRequestKey, _fixture.Password }
            });

        //Act
        var response = await _fixture.HttpClient.SendAsync(
            new HttpRequestMessage(HttpMethod.Delete, Constants.DeleteAdmin)
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

    [Fact, Order(11)]
    public async Task DeleteSelfAdmin_WrongPassword_Returns401()
    {
        //Arrange
        TestHelpers.SetAccessToken(_fixture.HttpClient, _fixture.AccessToken);

        var requestBody = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { Constants.UsernameRequestKey, _fixture.Username },
                { Constants.PasswordRequestKey, "wrong password" }
            });

        //Act
        var response = await _fixture.HttpClient.SendAsync(
            new HttpRequestMessage(HttpMethod.Delete, Constants.DeleteAdmin)
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

    [Fact, Order(12)]
    public async Task DeleteOtherAdmin_UsernameJwtMismatchNoKey_Returns403()
    {
        //Arrange
        var requestBody = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { Constants.UsernameRequestKey, _fixture.DifferentAdminUsername },
                { Constants.PasswordRequestKey, _fixture.Password }
            });

        //Act
        var response = await _fixture.HttpClient.SendAsync(
            new HttpRequestMessage(HttpMethod.Delete, Constants.DeleteAdmin)
            {
                Content = requestBody
            });

        //Assert
        var errors = TestHelpers.DeserializeHttpResponseBody<ErrorsDto>(response);

        var expectedErrors = new ErrorsDto(
            new List<ErrorDto>
            {
                new ErrorDto(403, $"You must provide an admin key when deleting another admin account: {_fixture.DifferentAdminUsername}")
            });

        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            errors.Should().BeEquivalentTo(expectedErrors);
        }
    }

    [Fact, Order(13)]
    public async Task DeleteOtherAdmin_UsernameJwtMismatchWrongKey_Returns403()
    {
        //Arrange
        var requestBody = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { Constants.UsernameRequestKey, _fixture.DifferentAdminUsername },
                { Constants.AdminKeyRequestKey, "wrong key" }
            });

        //Act
        var response = await _fixture.HttpClient.SendAsync(
            new HttpRequestMessage(HttpMethod.Delete, Constants.DeleteAdmin)
            {
                Content = requestBody
            });

        //Assert
        var errors = TestHelpers.DeserializeHttpResponseBody<ErrorsDto>(response);

        var expectedErrors = new ErrorsDto(
            new List<ErrorDto>
            {
                new ErrorDto(401, "Admin key is not correct")
            });

        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            errors.Should().BeEquivalentTo(expectedErrors);
        }
    }
    #endregion

    #region Deletion Happy Paths
    [Fact, Order(14)]
    public async Task DeleteOtherAdmin_UsernameMismatchCorrectKey_Returns200()
    {
        //Arrange
        var requestBody = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { Constants.UsernameRequestKey, _fixture.DifferentAdminUsername },
                { Constants.AdminKeyRequestKey, Constants.AdminKey }
            });

        //Act
        var response = await _fixture.HttpClient.SendAsync(
            new HttpRequestMessage(HttpMethod.Delete, Constants.DeleteAdmin)
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

    [Fact, Order(15)]
    public async Task DeleteUserAsAdmin_ValidUserNoAdminKey_Returns200()
    {
        //Arrange
        var requestBody = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { Constants.UsernameRequestKey, _fixture.DifferentUserUsername },
            });

        //Act
        var response = await _fixture.HttpClient.SendAsync(
            new HttpRequestMessage(HttpMethod.Delete, Constants.DeleteUserAsAdmin)
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

    [Fact, Order(16)]
    public async Task DeleteAdmin_UsernameMatch_Returns200()
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
            new HttpRequestMessage(HttpMethod.Delete, Constants.DeleteAdmin)
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
    #endregion

    #region Ensure Accounts Deleted
    [Fact, Order(17)]
    public async Task DeleteUserAsAdmin_UserNoLongerExists_Returns404()
    {
        //Arrange
        var requestBody = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { Constants.UsernameRequestKey, _fixture.DifferentUserUsername }
            });

        //Act
        var response = await _fixture.HttpClient.SendAsync(
            new HttpRequestMessage(HttpMethod.Delete, Constants.DeleteUserAsAdmin)
            {
                Content = requestBody
            });

        //Assert
        var errors = TestHelpers.DeserializeHttpResponseBody<ErrorsDto>(response);

        var expectedErrors = new ErrorsDto(
            new List<ErrorDto>
            {
                new ErrorDto(404, $"No user could be found with username: {_fixture.DifferentUserUsername}")
            });

        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            errors.Should().BeEquivalentTo(expectedErrors);
        }
    }

    [Fact, Order(18)]
    public async Task GetAdmin_DeletedAdmin_Returns404()
    {
        //Act
        var (statusCode, _, errors) = await TestHelpers.TryGetUser(_fixture.Username);

        //Assert
        var expectedErrors = new ErrorsDto(
            new List<ErrorDto>
            {
                new ErrorDto(404, $"No user could be found with username: {_fixture.Username}")
            });

        using (new AssertionScope())
        {
            statusCode.Should().Be(HttpStatusCode.NotFound);
            errors.Should().BeEquivalentTo(expectedErrors);
        }
    }

    [Fact, Order(19)]
    public async Task GetDifferentAdmin_DeletedByDifferentAdmin_Returns404()
    {
        //Act
        var (statusCode, _, errors) = await TestHelpers.TryGetUser(_fixture.DifferentAdminUsername);

        //Assert
        var expectedErrors = new ErrorsDto(
            new List<ErrorDto>
            {
                new ErrorDto(404, $"No user could be found with username: {_fixture.DifferentAdminUsername}")
            });

        using (new AssertionScope())
        {
            statusCode.Should().Be(HttpStatusCode.NotFound);
            errors.Should().BeEquivalentTo(expectedErrors);
        }
    }

    [Fact, Order(20)]
    public async Task GetUser_DeletedByAdmin_Returns404()
    {
        //Act
        var (statusCode, _, errors) = await TestHelpers.TryGetUser(_fixture.DifferentUserUsername);

        //Assert
        var expectedErrors = new ErrorsDto(
            new List<ErrorDto>
            {
                new ErrorDto(404, $"No user could be found with username: {_fixture.DifferentUserUsername}")
            });

        using (new AssertionScope())
        {
            statusCode.Should().Be(HttpStatusCode.NotFound);
            errors.Should().BeEquivalentTo(expectedErrors);
        }
    }
    #endregion
    #endregion
}
