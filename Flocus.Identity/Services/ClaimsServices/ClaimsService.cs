using Flocus.Identity.Interfaces;
using Flocus.Identity.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;

namespace Flocus.Identity.Services.ClaimsServices;

internal sealed class ClaimsService : IClaimsService
{
    private readonly string MissingClaimErrorMessage = "Could not find required claim from the JWT: {0}";

    public Claims GetClaimsFromUser(ClaimsPrincipal User)
    {
        var userClaimsDictionary = User.Claims.ToDictionary(x => x.Type, x => x.Value);

        var name = GetClaimByType(userClaimsDictionary, ClaimTypes.Name);
        var email = GetClaimByType(userClaimsDictionary, ClaimTypes.Email);
        var role = GetClaimByType(userClaimsDictionary, ClaimTypes.Role);
        var expiry = GetClaimByType(userClaimsDictionary, JwtRegisteredClaimNames.Exp);

        return new Claims(name, email, role, new DateTime(long.Parse(expiry)));
    }

    private string GetClaimByType(Dictionary<string, string> claimTypeValueDict, string claimType)
    {
        var claimValue = claimTypeValueDict.TryGetValue(claimType, out var claimTypeValue)
            ? claimTypeValue : throw new AuthenticationException(string.Format(MissingClaimErrorMessage, claimType));

        return claimValue;
    }
}
