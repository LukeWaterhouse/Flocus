using Flocus.Identity.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Flocus.Identity.Tests.Models;

public sealed class IdentitySettingsTests
{
    [Fact]
    public void CreateIdentitySettings_ValidConstructor_CreatesIdentitySettings()
    {
        // Arrange
        var signingKey = "123123123";
        var issuer = "issuer";
        var audience = "people";
        var adminKey = "12asdasd123";

        // Act
        var identitySettings = new IdentitySettings(
            signingKey,
            issuer,
            audience,
            adminKey);

        // Assert
        using (new AssertionScope())
        {
            identitySettings.SigningKey.Should().Be(signingKey);
            identitySettings.Issuer.Should().Be(issuer);
            identitySettings.Audience.Should().Be(audience);
        }
    }
}
