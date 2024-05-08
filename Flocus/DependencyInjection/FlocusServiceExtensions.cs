using Flocus.Domain.DependencyInjection;
using Flocus.Repository.DependencyInjection;
namespace Flocus.DependencyInjection;

public static class FlocusServiceExtensions
{
    public static IServiceCollection AddFlocusServices(this IServiceCollection services, IConfiguration configuration)
    {
        var capStructureDbConnectionString = configuration.GetSection("ConnectionStrings")["FlocusDb"];

        services.AddRepositoryServices(capStructureDbConnectionString);
        services.AddDomainServices();

        return services;
    }
}
