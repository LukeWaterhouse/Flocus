using AutoMapper;
using Flocus.Repository.Interfaces;
using Flocus.Repository.Mapping;
using Flocus.Repository.Services;
using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using Xunit;

namespace Flocus.Repository.Tests.Services.Repository;

public class DeleteUserTests
{
    private readonly IMapper _mapper;
    private readonly IUserSqlService _sqlQueryService;
    private readonly UserRepositoryService _repositoryService;

    public DeleteUserTests()
    {

        var mappingConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(UserMappingProfile));
        });

        _mapper = mappingConfig.CreateMapper();
        _sqlQueryService = Substitute.For<IUserSqlService>();
        _repositoryService = new UserRepositoryService(_mapper, _sqlQueryService);
    }

    [Fact]
    public async Task DeleteUser_ValidUserId_DoesNotThrowException()
    {
        // Arrange
        var userId = "userId1234";
        _sqlQueryService.DeleteUserWithRelatedTables(userId).Returns(true);

        // Act
        await _repositoryService.DeleteUserAsync(userId);

        // Assert
        await _sqlQueryService.Received(1).DeleteUserWithRelatedTables(userId);
    }

    [Fact]
    public async Task DeleteUser_InvalidUserId_ThrowsException()
    {
        // Arrange
        var userId = "userId1234";
        _sqlQueryService.DeleteUserWithRelatedTables(userId).Returns(false);

        // Act
        Exception exception = await Record.ExceptionAsync(async () =>
        {
            await _repositoryService.DeleteUserAsync(userId);
        });

        // Assert
        using (new AssertionScope())
        {
            exception.Should().BeOfType<Exception>();
            exception.Message.Should().Be("There was an error when deleting the user.");
            await _sqlQueryService.Received(1).DeleteUserWithRelatedTables(userId);
        }
    }
}
