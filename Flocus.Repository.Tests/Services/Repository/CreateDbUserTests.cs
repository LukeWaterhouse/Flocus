using AutoMapper;
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

public class CreateDbUserTests
{
    private readonly IMapper _mapper;
    private readonly IUserSqlService _sqlQueryService;
    private readonly UserRepositoryService _repositoryService;

    public CreateDbUserTests()
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
    public async Task CreateDbUser_ValidCredentials_ReturnsTrue()
    {
        // Arrange
        var username = "luke";
        var passwordHash = "passwordHash";
        var emailAddress = "luke@hotmail.com";
        var adminRights = false;

        var dbUser = new DbUser(
            Guid.NewGuid().ToString(),
            emailAddress,
            DateTime.UtcNow,
            username,
            passwordHash,
            adminRights);

        _sqlQueryService.GetUsersByUsernameOrEmailAsync(username, emailAddress).Returns(new List<DbUser>());
        _sqlQueryService.CreateUserAsync(Arg.Is<DbUser>(user =>
            user.Email_address == dbUser.Email_address &&
            EqualWithinFiveMinutes(user.Account_creation_date, dbUser.Account_creation_date) &&
            user.Username == dbUser.Username &&
            user.Password_hash == dbUser.Password_hash &&
            user.Admin_rights == dbUser.Admin_rights)).Returns(true);

        // Act
        await _repositoryService.CreateDbUserAsync(username, passwordHash, emailAddress, adminRights);

        // Assert
        using (new AssertionScope())
        {
            await _sqlQueryService.Received().GetUsersByUsernameOrEmailAsync(username, emailAddress);
            await _sqlQueryService.Received().CreateUserAsync(Arg.Is<DbUser>(user =>
                user.Email_address == dbUser.Email_address &&
                EqualWithinFiveMinutes(user.Account_creation_date, dbUser.Account_creation_date) &&
                user.Username == dbUser.Username &&
                user.Password_hash == dbUser.Password_hash &&
                user.Admin_rights == dbUser.Admin_rights));
        }
    }

    [Fact]
    public async Task CreateDbUser_IssueSaving_ThrowsException()
    {
        // Arrange
        var username = "luke";
        var passwordHash = "passwordHash";
        var emailAddress = "luke@hotmail.com";
        var adminRights = false;

        var dbUser = new DbUser(
            Guid.NewGuid().ToString(),
            emailAddress,
            DateTime.UtcNow,
            username,
            passwordHash,
            adminRights);

        _sqlQueryService.GetUsersByUsernameOrEmailAsync(username, emailAddress).Returns(new List<DbUser>());
        _sqlQueryService.CreateUserAsync(Arg.Is<DbUser>(user =>
           user.Email_address == dbUser.Email_address &&
           EqualWithinFiveMinutes(user.Account_creation_date, dbUser.Account_creation_date) &&
           user.Username == dbUser.Username &&
           user.Password_hash == dbUser.Password_hash &&
           user.Admin_rights == dbUser.Admin_rights)).Returns(false);

        // Act
        Exception exception = await Record.ExceptionAsync(async () =>
        {
            await _repositoryService.CreateDbUserAsync(username, passwordHash, emailAddress, adminRights);
        });

        // Assert
        using (new AssertionScope())
        {
            exception.Should().BeOfType<Exception>();
            exception.Message.Should().Be("There was an error when creating the user.");
            await _sqlQueryService.Received().GetUsersByUsernameOrEmailAsync(username, emailAddress);
            await _sqlQueryService.Received().CreateUserAsync(Arg.Is<DbUser>(user =>
                user.Email_address == dbUser.Email_address &&
                EqualWithinFiveMinutes(user.Account_creation_date, dbUser.Account_creation_date) &&
                user.Username == dbUser.Username &&
                user.Password_hash == dbUser.Password_hash &&
                user.Admin_rights == dbUser.Admin_rights));
        }
    }

    [Fact]
    public async Task CreateDbUser_DuplicateUsernameUser_ThrowsException()
    {
        // Arrange
        var username = "luke";
        var passwordHash = "passwordHash";
        var emailAddress = "luke@hotmail.com";
        var adminRights = false;

        var dbUser = new DbUser(
            Guid.NewGuid().ToString(),
            emailAddress,
            DateTime.UtcNow,
            username,
            passwordHash,
            adminRights);

        _sqlQueryService.GetUsersByUsernameOrEmailAsync(username, emailAddress).Returns(new List<DbUser> { dbUser });

        // Act
        Exception exception = await Record.ExceptionAsync(async () =>
        {
            await _repositoryService.CreateDbUserAsync(username, passwordHash, emailAddress, adminRights);
        });

        // Assert
        using (new AssertionScope())
        {
            exception.Should().BeOfType<DuplicateRecordException>();
            exception.Message.Should().Be("user already exists with username: luke");
            await _sqlQueryService.Received().GetUsersByUsernameOrEmailAsync(username, emailAddress);
            await _sqlQueryService.DidNotReceive().CreateUserAsync(Arg.Any<DbUser>());
        }
    }

    [Fact]
    public async Task CreateDbUser_DuplicateEmailUser_ThrowsException()
    {
        // Arrange
        var username = "luke";
        var passwordHash = "passwordHash";
        var emailAddress = "luke@hotmail.com";
        var adminRights = false;

        var dbUser = new DbUser(
            Guid.NewGuid().ToString(),
            emailAddress,
            DateTime.UtcNow,
            "differentUsername",
            passwordHash,
            adminRights);

        _sqlQueryService.GetUsersByUsernameOrEmailAsync(username, emailAddress).Returns(new List<DbUser> { dbUser });

        // Act
        Exception exception = await Record.ExceptionAsync(async () =>
        {
            await _repositoryService.CreateDbUserAsync(username, passwordHash, emailAddress, adminRights);
        });

        // Assert
        using (new AssertionScope())
        {
            exception.Should().BeOfType<DuplicateRecordException>();
            exception.Message.Should().Be("user already exists with email: luke@hotmail.com");
            await _sqlQueryService.Received().GetUsersByUsernameOrEmailAsync(username, emailAddress);
            await _sqlQueryService.DidNotReceive().CreateUserAsync(Arg.Any<DbUser>());
        }
    }

    [Fact]
    public async Task CreateDbUser_UsersFoundButDoNotMatch_ThrowsException()
    {
        // Arrange
        var username = "luke";
        var passwordHash = "passwordHash";
        var emailAddress = "luke@hotmail.com";
        var adminRights = false;

        var dbUser = new DbUser(
            Guid.NewGuid().ToString(),
            "differentEmail",
            DateTime.UtcNow,
            "differentUsername",
            passwordHash,
            adminRights);

        _sqlQueryService.GetUsersByUsernameOrEmailAsync(username, emailAddress).Returns(new List<DbUser> { dbUser });

        // Act
        Exception exception = await Record.ExceptionAsync(async () =>
        {
            await _repositoryService.CreateDbUserAsync(username, passwordHash, emailAddress, adminRights);
        });

        // Assert
        using (new AssertionScope())
        {
            exception.Should().BeOfType<Exception>();
            exception.Message.Should().Be($"Results found when searching users by {username} or {emailAddress} but were not caught.");
            await _sqlQueryService.Received().GetUsersByUsernameOrEmailAsync(username, emailAddress);
            await _sqlQueryService.DidNotReceive().CreateUserAsync(Arg.Any<DbUser>());
        }
    }

    private bool EqualWithinFiveMinutes(DateTime dateTime1, DateTime dateTime2)
    {
        TimeSpan difference = dateTime1 - dateTime2;
        double absoluteDifferenceMinutes = Math.Abs(difference.TotalMinutes);
        return absoluteDifferenceMinutes <= 5;
    }
}
