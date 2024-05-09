using Flocus.Domain.DependencyInjection;
using Flocus.Identity.DependencyInjection;
using Flocus.Middleware;
using Flocus.Repository.DependencyInjection;
namespace Flocus.DependencyInjection;

public static class FlocusServiceExtensions
{
    public static IServiceCollection AddFlocusServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRepositoryServices();
        services.AddDomainServices();
        services.AddIdentityServices();
        services.AddTransient<ExceptionMiddleware>();

        return services;
    }
}
