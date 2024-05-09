using Flocus.Identity.Interfaces;
using Flocus.Identity.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Flocus.Identity.DependencyInjection;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddScoped<IIdentityService, IdentityService>();
        return services;
    }
}
