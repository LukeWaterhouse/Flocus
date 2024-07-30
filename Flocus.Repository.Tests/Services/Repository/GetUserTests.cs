using AutoMapper;
using Flocus.Domain.Models;
using Flocus.Repository.Exceptions;
using Flocus.Repository.Interfaces;
using Flocus.Repository.Mapping;
using Flocus.Repository.Models;
using Flocus.Repository.Services;
using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using Xunit;

namespace Flocus.Repository.Tests.Services.Repository;

public class GetUserTests
{
    private readonly IMapper _mapper;
    private readonly IDbConnectionService _dbConnectionService;
    private readonly IUserSqlService _sqlQueryService;
    private readonly UserRepositoryService _repositoryService;

    public GetUserTests()
    {

        var mappingConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(UserMappingProfile));
        });

        _mapper = mappingConfig.CreateMapper();
        _dbConnectionService = Substitute.For<IDbConnectionService>();
        _sqlQueryService = Substitute.For<IUserSqlService>();
        _repositoryService = new UserRepositoryService(_mapper, _sqlQueryService);
    }

    [Fact]
    public async Task GetUserTests_ValidUsername_ReturnsUser()
    {
        // Arrange
        var username = "luke";

        _sqlQueryService.GetUsersByUsernameAsync(username).Returns(
            new List<DbUser>
            {
                new DbUser(
                    "123",
                    "luke@hotmail.com",
                    new DateTime(2024, 05, 13),
                    username,
                    "hash-123",
                    true)
            });


        // Act
        var result = await _repositoryService.GetUserAsync(username);

        // Assert
        var expectedUser = new User(
            "123",
            "luke@hotmail.com",
            new DateTime(2024, 05, 13),
            username,
            true,
            "hash-123");

        using (new AssertionScope())
        {
            result.Should().BeEquivalentTo(expectedUser);
            await _sqlQueryService.Received().GetUsersByUsernameAsync(username);
        }
    }

    [Fact]
    public async Task GetUserTests_NoMatchingUser_ThrowsException()
    {
        // Arrange
        var username = "luke";
        _sqlQueryService.GetUsersByUsernameAsync(username).Returns(new List<DbUser>());

        // Act
        Exception exception = await Record.ExceptionAsync(async () =>
        {
            var result = await _repositoryService.GetUserAsync(username);
        });

        // Assert
        using (new AssertionScope())
        {
            exception.Should().BeOfType<RecordNotFoundException>();
            exception.Message.Should().Be("No user could be found with username: luke");
            await _sqlQueryService.Received().GetUsersByUsernameAsync(username);
        }
    }

    [Fact]
    public async Task GetUserTests_InvalidNumberUsersRetrieved_ThrowsException()
    {
        // Arrange
        var username = "luke";

        _sqlQueryService.GetUsersByUsernameAsync(username).Returns(
            new List<DbUser>
            {
                new DbUser(
                    "123",
                    "luke@hotmail.com",
                    new DateTime(2024, 05, 13),
                    username,
                    "hash-123",
                    true),
                new DbUser(
                    "234",
                    "luke2@hotmail.com",
                    new DateTime(2024, 05, 13),
                    username,
                    "hash-234",
                    false)
            });

        // Act
        Exception exception = await Record.ExceptionAsync(async () =>
        {
            var result = await _repositoryService.GetUserAsync(username);
        });

        // Assert
        using (new AssertionScope())
        {
            exception.Should().BeOfType<Exception>();
            exception.Message.Should().Be("invalid number of users with username: luke, found 2");
            await _sqlQueryService.Received().GetUsersByUsernameAsync(username);
        }
    }
}
