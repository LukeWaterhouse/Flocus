using Flocus.Identity.Interfaces;
using Flocus.Identity.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Flocus.Identity.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddScoped<IIdentityService, IdentityService>();
        return services;
    }
}
