using Flocus.Domain.Interfaces;
using Flocus.Domain.Services.UserServices;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Flocus.Domain.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class DomainServiceExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}
