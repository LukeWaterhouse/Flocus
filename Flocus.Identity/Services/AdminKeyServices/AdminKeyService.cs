using Flocus.Identity.Interfaces.AdminKeyInterfaces;
using Flocus.Identity.Models;
using System.Security.Authentication;

namespace Flocus.Identity.Services.AdminKeyServices;

public sealed class AdminKeyService : IAdminKeyService
{
    private readonly IdentitySettings _identitySettings;

    private readonly string AdminKeyIncorrectMessage = "Admin key is not correct";

    public AdminKeyService(IdentitySettings identitySettings)
    {
        _identitySettings = identitySettings;
    }

    public void CheckAdminKeyCorrect(string adminKey)
    {
        if (adminKey != _identitySettings.AdminKey)
        {
            throw new AuthenticationException(AdminKeyIncorrectMessage);
        }
    }
}
