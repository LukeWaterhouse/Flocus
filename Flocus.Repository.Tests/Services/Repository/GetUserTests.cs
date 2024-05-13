using AutoMapper;
using Flocus.Domain.Models;
using Flocus.Repository.Exceptions;
using Flocus.Repository.Interfaces;
using Flocus.Repository.Mapping;
using Flocus.Repository.Models;
using Flocus.Repository.Services;
using FluentAssertions;
using NSubstitute;
using System.Net.Mail;
using Xunit;

namespace Flocus.Repository.Tests.Services.Repository;

public class GetUserTests
{
    private readonly IMapper _mapper;
    private readonly IDbConnectionService _dbConnectionService;
    private readonly ISqlQueryService _sqlQueryService;
    private readonly RepositoryService _repositoryService;

    public GetUserTests()
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
    public async Task GetUserTests_ValidUsername_ReturnsUSer()
    {
        //Arrange
        var username = "luke";

        _sqlQueryService.GetUsersByUsernameAsync(username).Returns(
            new List<DbUser>
            {
                new DbUser()
                {
                    Client_id = "123",
                    Email_address = "luke@hotmail.com",
                    Account_creation_date = new DateTime(2024, 05, 13),
                    Username = username,
                    Password_hash = "hash-123",
                    Admin_rights = true
                }
            });;


        //Act
        var result = await _repositoryService.GetUserAsync(username);

        //Assert
        var expectedUser = new User()
        {
            ClientId = "123",
            EmailAddress = "luke@hotmail.com",
            CreatedAt = new DateTime(2024, 05, 13),
            Username = "luke",
            PasswordHash = "hash-123",
            IsAdmin = true
        };

        result.Should().BeEquivalentTo(expectedUser);
        await _sqlQueryService.Received().GetUsersByUsernameAsync(username);
    }

    [Fact]
    public async Task GetUserTests_NoMatchingUser_ThrowsException()
    {
        //Arrange
        var username = "luke";

        _sqlQueryService.GetUsersByUsernameAsync(username).Returns(new List<DbUser>());


        //Act
        Exception exception = await Record.ExceptionAsync(async () =>
        {
            var result = await _repositoryService.GetUserAsync(username);
        });

        //Assert
        exception.Should().BeOfType<RecordNotFoundException>();
        exception.Message.Should().Be("No user could be found with username: luke");
        await _sqlQueryService.Received().GetUsersByUsernameAsync(username);
    }

    [Fact]
    public async Task GetUserTests_InvalidNumberUsersRetrieved_ThrowsException()
    {
        //Arrange
        var username = "luke";

        _sqlQueryService.GetUsersByUsernameAsync(username).Returns(
            new List<DbUser>
            {
                new DbUser()
                {
                    Client_id = "123",
                    Email_address = "luke@hotmail.com",
                    Account_creation_date = new DateTime(2024, 05, 13),
                    Username = username,
                    Password_hash = "hash-123",
                    Admin_rights = true
                },
                new DbUser()
                {
                    Client_id = "234",
                    Email_address = "luke2@hotmail.com",
                    Account_creation_date = new DateTime(2024, 05, 13),
                    Username = username,
                    Password_hash = "hash-234",
                    Admin_rights = false
                }
            }); ;


        //Act
        Exception exception = await Record.ExceptionAsync(async () =>
        {
            var result = await _repositoryService.GetUserAsync(username);
        });

        //Assert
        exception.Should().BeOfType<Exception>();
        exception.Message.Should().Be("invalid number of users with username: luke, found 2");
        await _sqlQueryService.Received().GetUsersByUsernameAsync(username);
    }
}
