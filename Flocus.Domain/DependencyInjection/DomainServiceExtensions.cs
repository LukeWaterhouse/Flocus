using Flocus.Domain.Interfaces;
using Flocus.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Flocus.Domain.DependencyInjection;

public static class DomainServiceExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}
