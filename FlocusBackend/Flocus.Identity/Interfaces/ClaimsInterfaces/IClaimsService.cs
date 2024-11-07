using Flocus.Identity.Models;
using System.Security.Claims;

namespace Flocus.Identity.Interfaces;

public interface IClaimsService
{
    Claims GetClaimsFromUser(ClaimsPrincipal User);
}
