using Flocus.Domain.Interfaces;
using Flocus.Identity.Interfaces;
using Flocus.Identity.Interfaces.AdminKeyInterfaces;
using Flocus.Identity.Interfaces.RegisterInterfaces;
using Flocus.Identity.Models;
using Flocus.Identity.Services.RegistrationServices;
using FluentAssertions.Execution;
using NSubstitute;
using Xunit;
using BC = BCrypt.Net.BCrypt;

namespace Flocus.Identity.Tests.Services.RegistrationServicesTests;

public sealed class RegistrationServiceTests
{
    private readonly IRegistrationValidationService _registerValidationServiceMock;
    private readonly IUserRepositoryService _userRepositoryServiceMock;
    private readonly IAdminKeyService _adminKeyServiceMock;

    private readonly IRegistrationService _registrationService;

    public RegistrationServiceTests()
    {
        _userRepositoryServiceMock = Substitute.For<IUserRepositoryService>();
        _registerValidationServiceMock = Substitute.For<IRegistrationValidationService>();
        _adminKeyServiceMock = Substitute.For<IAdminKeyService>();

        _registrationService = new RegistrationService(_registerValidationServiceMock, _userRepositoryServiceMock, _adminKeyServiceMock);
    }

    [Fact]
    public async Task RegisterAsync_WithValidParameters_CallsRequiredServices()
    {
        // Arrange
        var username = "luke";
        var password = "hashedPassword";
        var isAdmin = false;
        var email = "luke@hotmail.com";
        var key = "key";

        var registrationModel = new RegistrationModel(
            username,
            password,
            email,
            isAdmin,
            key);

        // Act
        await _registrationService.RegisterAsync(registrationModel);

        // Assert
        using (new AssertionScope())
        {
            await _userRepositoryServiceMock.Received(1).CreateDbUserAsync(
            username,
            Arg.Is<string>(hash => VerifyPassword(password, hash)),
            email,
            isAdmin);

            _registerValidationServiceMock.Received(1).InputValidateRegistrationModel(registrationModel);
            _adminKeyServiceMock.DidNotReceiveWithAnyArgs().CheckAdminKeyCorrect(Arg.Any<string>());
        }
    }

    [Fact]
    public async Task RegisterAsync_WithValidAdmin_CallsRequiredServices()
    {
        // Arrange
        var username = "luke";
        var password = "hashedPassword";
        var isAdmin = true;
        var email = "luke@hotmail.com";
        var key = "adminKey";

        var registrationModel = new RegistrationModel(
            username,
            password,
            email,
            isAdmin,
            key);

        // Act
        await _registrationService.RegisterAsync(registrationModel);

        // Assert
        using (new AssertionScope())
        {
            await _userRepositoryServiceMock.Received().CreateDbUserAsync(
            username,
            Arg.Is<string>(hash => VerifyPassword(password, hash)),
            email,
            isAdmin);

            _registerValidationServiceMock.Received(1).InputValidateRegistrationModel(registrationModel);
            _adminKeyServiceMock.Received(1).CheckAdminKeyCorrect(key);
        }
    }

    bool VerifyPassword(string password, string input)
    {
        return BC.Verify(password, input);
    }
}
