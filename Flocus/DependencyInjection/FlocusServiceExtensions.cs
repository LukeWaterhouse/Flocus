using Flocus.Domain.DependencyInjection;
using Flocus.Identity.DependencyInjection;
using Flocus.Mapping;
using Flocus.Middleware;
using Flocus.Repository.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Flocus.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class FlocusServiceExtensions
{
    public static IServiceCollection AddFlocusServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRepositoryServices();
        services.AddDomainServices();
        services.AddIdentityServices(configuration);
        services.AddTransient<ExceptionMiddleware>();
        services.AddAutoMapper(Assembly.GetAssembly(typeof(UserDtoMappingProfile)));
        return services;
    }
}
