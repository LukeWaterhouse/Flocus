using Flocus.Identity.Interfaces;
using Flocus.Identity.Models;
using Flocus.Identity.Services.ClaimsServices;
using FluentAssertions;
using FluentAssertions.Execution;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using Xunit;

namespace Flocus.Identity.Tests.Services.ClaimsServicesTests;

public class ClaimsServiceTests
{
    private readonly IClaimsService _claimsService;

    public ClaimsServiceTests()
    {
        _claimsService = new ClaimsService();
    }

    [Fact]
    public void GetClaimsFromUser_ValidUser_ReturnsClaims()
    {
        // Arrange
        var name = "luke";
        var email = "luke@hotmail.com";
        var role = "Admin";
        var exp = "1722425199";

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Exp, exp)
        };

        var claimsIdentity = new ClaimsIdentity(claims);
        var user = new ClaimsPrincipal(claimsIdentity);

        // Act
        var result = _claimsService.GetClaimsFromUser(user);

        // Assert
        var expectedClaims = new Claims(name, email, role, new DateTime(2024, 07, 31, 11, 26, 39));

        using (new AssertionScope())
        {
            result.Should().BeEquivalentTo(expectedClaims);
        }
    }

    [Fact]
    public void GetClaimsFromUser_UserMissingRoleClaim_ThrowsException()
    {
        // Arrange
        var name = "luke";
        var email = "luke@hotmail.com";
        var exp = "1722425199";

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Email, email),
            new Claim(JwtRegisteredClaimNames.Exp, exp)
        };

        var claimsIdentity = new ClaimsIdentity(claims);
        var user = new ClaimsPrincipal(claimsIdentity);

        // Act
        Exception exception = Record.Exception(() =>
        {
            var result = _claimsService.GetClaimsFromUser(user);
        });

        // Assert
        using (new AssertionScope())
        {
            exception.Should().BeOfType<AuthenticationException>();
            exception.Message.Should().Be($"Could not find required claim from the JWT: {ClaimTypes.Role}");
        }
    }

    [Fact]
    public void GetClaimsFromUser_UserMissingNameClaim_ThrowsException()
    {
        // Arrange
        var email = "luke@hotmail.com";
        var role = "Admin";
        var exp = "1722425199";

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Exp, exp)
        };

        var claimsIdentity = new ClaimsIdentity(claims);
        var user = new ClaimsPrincipal(claimsIdentity);

        // Act
        Exception exception = Record.Exception(() =>
        {
            var result = _claimsService.GetClaimsFromUser(user);
        });

        // Assert
        using (new AssertionScope())
        {
            exception.Should().BeOfType<AuthenticationException>();
            exception.Message.Should().Be($"Could not find required claim from the JWT: {ClaimTypes.Name}");
        }
    }

    [Fact]
    public void GetClaimsFromUser_UserMissingEmailClaim_ThrowsException()
    {
        // Arrange
        var name = "luke";
        var role = "Admin";
        var exp = "1722425199";

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Exp, exp)
        };

        var claimsIdentity = new ClaimsIdentity(claims);
        var user = new ClaimsPrincipal(claimsIdentity);

        // Act
        Exception exception = Record.Exception(() =>
        {
            var result = _claimsService.GetClaimsFromUser(user);
        });

        // Assert
        using (new AssertionScope())
        {
            exception.Should().BeOfType<AuthenticationException>();
            exception.Message.Should().Be($"Could not find required claim from the JWT: {ClaimTypes.Email}");
        }
    }

    [Fact]
    public void GetClaimsFromUser_UserMissingExpClaim_ThrowsException()
    {
        // Arrange
        var name = "luke";
        var email = "luke@hotmail.com";
        var role = "Admin";

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
        };

        var claimsIdentity = new ClaimsIdentity(claims);
        var user = new ClaimsPrincipal(claimsIdentity);

        // Act
        Exception exception = Record.Exception(() =>
        {
            var result = _claimsService.GetClaimsFromUser(user);
        });

        // Assert
        using (new AssertionScope())
        {
            exception.Should().BeOfType<AuthenticationException>();
            exception.Message.Should().Be($"Could not find required claim from the JWT: {JwtRegisteredClaimNames.Exp}");
        }
    }
}
