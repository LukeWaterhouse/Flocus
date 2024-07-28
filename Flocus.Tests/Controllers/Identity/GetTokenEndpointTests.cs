﻿using AutoMapper;
using Flocus.Controllers;
using Flocus.Identity.Interfaces;
using Flocus.Mapping;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Flocus.Tests.Controllers.Identity;

public class GetTokenEndpointTests
{
    private readonly ILogger<IdentityController> _loggerMock;
    private readonly IRemoveAccountService _identityServiceMock;
    private readonly IClaimsService _claimsServiceMock;
    private readonly IMapper _mapper;
    private readonly IdentityController _identityController;

    public GetTokenEndpointTests()
    {
        _loggerMock = Substitute.For<ILogger<IdentityController>>();
        _identityServiceMock = Substitute.For<IRemoveAccountService>();
        _claimsServiceMock = Substitute.For<IClaimsService>();

        var mappingConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(RegistrationModelMappingProfile));
        });
        _mapper = mappingConfig.CreateMapper();

        _identityController = new IdentityController(_loggerMock, _mapper, _claimsServiceMock, _identityServiceMock);
    }

    [Fact]
    public async Task GetTokenAsync_ValidRequest_ReturnsOkWithToken()
    {
        //Arrange
        var username = "luke";
        var password = "rollo123";

        _identityServiceMock.GetAuthTokenAsync(username, password).Returns(Task.FromResult("token"));

        //Act
        var result = await _identityController.GetTokenAsync(username, password, CancellationToken.None);

        //Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            var body = ((OkObjectResult)result).Value!;
            body.Should().BeEquivalentTo("token");
        }
    }

    [Fact]
    public async Task GetTokenAsync_IssueWithGetToken_ReturnsInternalServerError()
    {
        //Arrange
        var username = "luke";
        var password = "rollo123";

        _identityServiceMock.GetAuthTokenAsync(username, password).Returns(Task.FromResult<string>(null));

        //Act
        var result = await _identityController.GetTokenAsync(username, password, CancellationToken.None);

        //Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            var objectResult = (ObjectResult)result;
            objectResult.Value.Should().Be("Error generating token");
            objectResult.StatusCode.Should().Be(500);
        }
    }
}