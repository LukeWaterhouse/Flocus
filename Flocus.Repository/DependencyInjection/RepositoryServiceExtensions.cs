using Flocus.Domain.Interfaces;
using Flocus.Repository.Interfaces;
using Flocus.Repository.Mapping;
using Flocus.Repository.Services;
using Flocus.Repository.Services.Sql.Connection;
using Flocus.Repository.Services.SqlServices;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Flocus.Repository.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class RepositoryServiceExtensions
{
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<IUserRepositoryService, UserRepositoryService>();
        services.AddScoped<IDbConnectionService, DbConnectionService>();
        services.AddScoped<IUserSqlService, UserSqlService>();
        services.AddAutoMapper(Assembly.GetAssembly(typeof(UserMappingProfile)));

        return services;
    }
}
