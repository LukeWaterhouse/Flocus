using Flocus.Domain.DependencyInjection;
using Flocus.Repository.DependencyInjection;

namespace Flocus.DependencyInjection;

public static class FlocusServiceExtensions
{
    public static IServiceCollection AddFlocusServices(this IServiceCollection services)
    {
        services.AddRepositoryServices();
        services.AddDomainServices();

        return services;
    }
}
