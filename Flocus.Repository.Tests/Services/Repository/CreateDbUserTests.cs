using AutoMapper;
using Flocus.Repository.Exceptions;
using Flocus.Repository.Interfaces;
using Flocus.Repository.Mapping;
using Flocus.Repository.Models;
using Flocus.Repository.Services;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Flocus.Repository.Tests.Services.Repository;

public class CreateDbUserTests
{
    private readonly IMapper _mapper;
    private readonly IDbConnectionService _dbConnectionService;
    private readonly ISqlQueryService _sqlQueryService;
    private readonly RepositoryService _repositoryService;

    public CreateDbUserTests()
    {

        var mappingConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(UserMappingProfile));
        });

        _mapper = mappingConfig.CreateMapper();
        _dbConnectionService = Substitute.For<IDbConnectionService>();
        _sqlQueryService = Substitute.For<ISqlQueryService>();
        _repositoryService = new RepositoryService(_mapper, _dbConnectionService, _sqlQueryService);
    }

    [Fact]
    public async Task CreateDbUser_ValidCredentials_ReturnsTrue()
    {
        //Arrange
        var username = "luke";
        var passwordHash = "passwordHash";
        var emailAddress = "luke@hotmail.com";
        var adminRights = false;

        var dbUser = new DbUser
        {
            Client_id = Guid.NewGuid().ToString(),
            Email_address = emailAddress,
            Account_creation_date = DateTime.Now,
            Username = username,
            Password_hash = passwordHash,
            Admin_rights = adminRights
        };

        _sqlQueryService.GetUsersByUsernameAsync(username).Returns(new List<DbUser>());
        _sqlQueryService.CreateUserAsync(Arg.Is<DbUser>(user =>
            user.Email_address == dbUser.Email_address &&
            EqualWithinFiveMinutes(user.Account_creation_date, dbUser.Account_creation_date) &&
            user.Username == dbUser.Username &&
            user.Password_hash == dbUser.Password_hash &&
            user.Admin_rights == dbUser.Admin_rights)).Returns(true);

        //Act
        var result = await _repositoryService.CreateDbUserAsync(username, passwordHash, emailAddress, adminRights);

        //Assert
        Assert.True(result);
        await _sqlQueryService.Received().GetUsersByUsernameAsync(username);
        await _sqlQueryService.Received().CreateUserAsync(Arg.Is<DbUser>(user =>
            user.Email_address == dbUser.Email_address &&
            EqualWithinFiveMinutes(user.Account_creation_date, dbUser.Account_creation_date) &&
            user.Username == dbUser.Username &&
            user.Password_hash == dbUser.Password_hash &&
            user.Admin_rights == dbUser.Admin_rights));
    }

    [Fact]
    public async Task CreateDbUser_IssueSaving_ReturnsFalse()
    {
        //Arrange

        var username = "luke";
        var passwordHash = "passwordHash";
        var emailAddress = "luke@hotmail.com";
        var adminRights = false;

        var dbUser = new DbUser
        {
            Client_id = Guid.NewGuid().ToString(),
            Email_address = emailAddress,
            Account_creation_date = DateTime.Now,
            Username = username,
            Password_hash = passwordHash,
            Admin_rights = adminRights
        };

        _sqlQueryService.GetUsersByUsernameAsync(username).Returns(new List<DbUser>());
        _sqlQueryService.CreateUserAsync(Arg.Is<DbUser>(user =>
           user.Email_address == dbUser.Email_address &&
           EqualWithinFiveMinutes(user.Account_creation_date, dbUser.Account_creation_date) &&
           user.Username == dbUser.Username &&
           user.Password_hash == dbUser.Password_hash &&
           user.Admin_rights == dbUser.Admin_rights)).Returns(false);

        //Act
        var result = await _repositoryService.CreateDbUserAsync(username, passwordHash, emailAddress, adminRights);

        //Assert
        Assert.False(result);
        await _sqlQueryService.Received().GetUsersByUsernameAsync(username);
        await _sqlQueryService.Received().CreateUserAsync(Arg.Is<DbUser>(user =>
            user.Email_address == dbUser.Email_address &&
            EqualWithinFiveMinutes(user.Account_creation_date, dbUser.Account_creation_date) &&
            user.Username == dbUser.Username &&
            user.Password_hash == dbUser.Password_hash &&
            user.Admin_rights == dbUser.Admin_rights));
    }

    [Fact]
    public async Task CreateDbUser_DuplicateUser_ThrowsException()
    {
        //Arrange
        var username = "luke";
        var passwordHash = "passwordHash";
        var emailAddress = "luke@hotmail.com";
        var adminRights = false;

        var dbUser = new DbUser
        {
            Client_id = Guid.NewGuid().ToString(),
            Email_address = emailAddress,
            Account_creation_date = DateTime.Now,
            Username = username,
            Password_hash = passwordHash,
            Admin_rights = adminRights
        };

        _sqlQueryService.GetUsersByUsernameAsync(username).Returns(new List<DbUser> { new DbUser() });

        //Act
        Exception exception = await Record.ExceptionAsync(async () =>
        {
            var result = await _repositoryService.CreateDbUserAsync(username, passwordHash, emailAddress, adminRights);
        });

        //Assert
        exception.Should().BeOfType<DuplicateRecordException>();
        exception.Message.Should().Be("user already exists with username: luke");
        await _sqlQueryService.Received().GetUsersByUsernameAsync(username);
        await _sqlQueryService.DidNotReceive().CreateUserAsync(Arg.Any<DbUser>());
    }

    public static bool EqualWithinFiveMinutes(DateTime dateTime1, DateTime dateTime2)
    {
        TimeSpan difference = dateTime1 - dateTime2;
        double absoluteDifferenceMinutes = Math.Abs(difference.TotalMinutes);
        return absoluteDifferenceMinutes <= 5;
    }
}
