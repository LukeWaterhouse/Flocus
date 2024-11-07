using AutoMapper;
using Flocus.Controllers;
using Flocus.Domain.Interfaces;
using Flocus.Domain.Models;
using Flocus.Identity.Interfaces;
using Flocus.Identity.Models;
using Flocus.Mapping;
using Flocus.Models.ReturnModels.UserReturnModels;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Security.Claims;
using Xunit;

namespace Flocus.Tests.ControllerTests.UserControllerTests;

public sealed class GetUserEndpointTests
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

    [Theory]
    [InlineData(true, "Admin")]
    [InlineData(false, "User")]
    public async Task GetUserAsync_ValidUser_ReturnsOk(bool isAdmin, string claimsRole)
    {
        // Arrange
        var username = "lukosparta123";
        var email = "lukewwaterhouse@hotmail.com";
        var createdAtDate = DateTime.UtcNow;
        var clientId = "123123";

        var claims = new Claims(username, email, claimsRole, DateTime.UtcNow);
        _claimsServiceMock.GetClaimsFromUser(Arg.Any<ClaimsPrincipal>()).Returns(claims);

        var user = new User(clientId, email, createdAtDate, username, isAdmin, "passwordHash");
        _userServiceMock.GetUserAsync(username).Returns(user);

        // Act
        var result = await _userController.GetUserAsync();

        // Assert
        var expectedResponse = new UserSensitiveInfoDto(clientId, email, createdAtDate, username, isAdmin);

        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            var body = ((OkObjectResult)result).Value!;
            body.Should().BeEquivalentTo(expectedResponse);
        }
    }
}
