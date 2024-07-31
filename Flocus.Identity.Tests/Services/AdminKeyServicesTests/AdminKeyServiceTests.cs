using Flocus.Identity.Models;
using Flocus.Identity.Services.AdminKeyServices;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Security.Authentication;
using Xunit;

namespace Flocus.Identity.Tests.Services.AdminKeyServicesTests;

public sealed class AdminKeyServiceTests
{
    private readonly IdentitySettings _identitySettings;
    private readonly AdminKeyService _adminKeyService;

    private readonly string CorrectAdminKey = "adminKey";

    public AdminKeyServiceTests()
    {
        _identitySettings = new IdentitySettings("signingKey", "issuer", "audience", CorrectAdminKey);
        _adminKeyService = new AdminKeyService(_identitySettings);
    }

    [Fact]
    public void CheckAdminKeyCorrect_CorrectAdminKey_DoesNotThrowException()
    {
        // Arrange
        var inputAdminKey = "adminKey";

        // Act
        _adminKeyService.CheckAdminKeyCorrect(inputAdminKey);
    }

    [Fact]
    public void CheckAdminKeyCorrect_IncorrectAdminKey_ThrowsException()
    {
        // Arrange
        var inputAdminKey = "incorrect adminKey";

        // Act
        Exception exception = Record.Exception(() =>
        {
            _adminKeyService.CheckAdminKeyCorrect(inputAdminKey);
        });

        // Assert
        using (new AssertionScope())
        {
            exception.Should().BeOfType<AuthenticationException>();
            exception.Message.Should().Be("Admin key is not correct");
        }
    }
}
