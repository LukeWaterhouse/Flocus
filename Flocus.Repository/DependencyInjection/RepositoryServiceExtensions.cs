using Flocus.Domain.Interfacesl;
using Flocus.Repository.Interfaces;
using Flocus.Repository.Mapping;
using Flocus.Repository.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Flocus.Repository.DependencyInjection;

public static class RepositoryServiceExtensions
{
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<IRepositoryService, RepositoryService>();
        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<ISqlQueryFactory, SqlQueryFactory>();
        services.AddAutoMapper(Assembly.GetAssembly(typeof(UserMappingProfile)));

        return services;
    }
}
