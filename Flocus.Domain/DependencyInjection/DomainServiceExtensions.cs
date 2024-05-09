using Microsoft.Extensions.DependencyInjection;

namespace Flocus.Domain.DependencyInjection;

public static class DomainServiceExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        return services;
    }
}
