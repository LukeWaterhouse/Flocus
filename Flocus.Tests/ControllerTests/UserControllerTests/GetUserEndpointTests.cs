using AutoMapper;
using Flocus.Controllers;
using Flocus.Domain.Interfaces;
using Flocus.Domain.Models;
using Flocus.Identity.Interfaces;
using Flocus.Identity.Models;
using Flocus.Mapping;
using Flocus.Models.ReturnModels;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Security.Claims;
using Xunit;

namespace Flocus.Tests.ControllerTests.UserControllerTests;

public class GetUserEndpointTests
{
    private readonly ILogger<UserController> _loggerMock;
    private readonly IUserService _userServiceMock;
    private readonly IClaimsService _claimsServiceMock;
    private readonly IMapper _mapper;

    private readonly UserController _userController;

    public GetUserEndpointTests()
    {
        _loggerMock = Substitute.For<ILogger<UserController>>();
        _userServiceMock = Substitute.For<IUserService>();
        _claimsServiceMock = Substitute.For<IClaimsService>();

        var mappingConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(UserDtoMappingProfile));
        });
        _mapper = mappingConfig.CreateMapper();

        _userController = new UserController(_loggerMock, _userServiceMock, _claimsServiceMock, _mapper);
    }

    [Fact]
    public async Task GetUserAsync_UserRoleAndNoInputUsername_ReturnsOk()
    {
        // Arrange
        var username = "lukosparta123";
        var email = "lukewwaterhouse@hotmail.com";
        var role = "User";

        var createdAtDate = DateTime.UtcNow;
        var clientId = "123123";

        var claims = new Claims(username, email, role, DateTime.UtcNow);
        _claimsServiceMock.GetClaimsFromUser(Arg.Any<ClaimsPrincipal>()).Returns(claims);

        var user = new User(clientId, email, createdAtDate, username, false, "passwordhash");
        _userServiceMock.GetUserAsync(username).Returns(user);

        // Act
        var result = await _userController.GetUserAsync(username);

        // Assert
        var expectedResponse = new UserDto(email, createdAtDate, username);

        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            var body = ((OkObjectResult)result).Value!;
            body.Should().BeEquivalentTo(expectedResponse);
        }
    }

    [Fact]
    public async Task GetUserAsync_AdminRoleAndInputUsernameMismatchesClaimsUsername_ReturnsOk()
    {
        // Arrange
        var inputUsername = "lukosparta123";
        var email = "lukewwaterhouse@hotmail.com";
        var role = "Admin";

        var claimsUsername = "kratos1998";

        var createdAtDate = DateTime.UtcNow;
        var clientId = "123123";

        var claims = new Claims(claimsUsername, email, role, DateTime.UtcNow);
        _claimsServiceMock.GetClaimsFromUser(Arg.Any<ClaimsPrincipal>()).Returns(claims);

        var user = new User(clientId, email, createdAtDate, inputUsername, false, "passwordhash");
        _userServiceMock.GetUserAsync(inputUsername).Returns(user);

        // Act
        var result = await _userController.GetUserAsync(inputUsername);

        // Assert
        var expectedResponse = new UserDto(email, createdAtDate, inputUsername);

        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            var body = ((OkObjectResult)result).Value!;
            body.Should().BeEquivalentTo(expectedResponse);
        }
    }

    [Fact]
    public async Task GetUserAsync_NonAdminRoleAndInputUsernameMismatchesClaimsUsername_ThrowsException()
    {
        // Arrange
        var inputUsername = "lukosparta123";
        var email = "lukewwaterhouse@hotmail.com";
        var role = "User";

        var claimsUsername = "kratos1998";

        var createdAtDate = DateTime.UtcNow;
        var clientId = "123123";

        var claims = new Claims(claimsUsername, email, role, DateTime.UtcNow);
        _claimsServiceMock.GetClaimsFromUser(Arg.Any<ClaimsPrincipal>()).Returns(claims);

        var user = new User(clientId, email, createdAtDate, inputUsername, false, "passwordhash");
        _userServiceMock.GetUserAsync(inputUsername).Returns(user);

        //Act
        Exception exception = await Record.ExceptionAsync(async () =>
        {
            var result = await _userController.GetUserAsync(inputUsername);
        });

        // Assert
        var expectedResponse = new UserDto(email, createdAtDate, inputUsername);

        using (new AssertionScope())
        {
            exception.Should().BeOfType<UnauthorizedAccessException>();
            exception.Message.Should().Be("Must be 'Admin' to access other users.");
            await _userServiceMock.DidNotReceiveWithAnyArgs().GetUserAsync(Arg.Any<string>());
        }
    }
}
