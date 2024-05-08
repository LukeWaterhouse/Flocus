using Flocus.Domain.Interfacesl;
using Flocus.Repository.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Flocus.Repository.DependencyInjection;

public static class RepositoryServiceExtensions
{
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<IRepositoryService, RepositoryService>();
        return services;
    }
}
