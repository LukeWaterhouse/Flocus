using Flocus.Identity.Interfaces;
using Flocus.Identity.Models;
using Flocus.Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Flocus.Identity.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        var identitySettingsKey = "AppSettings:Identity";
        var signingKey = configuration[$"{identitySettingsKey}:SigningKey"];
        var issuer = configuration[$"{identitySettingsKey}:Issuer"];
        var audience = configuration[$"{identitySettingsKey}:Audience"];
        var adminKey = configuration[$"{identitySettingsKey}:AdminKey"];

        var appSettings = new IdentitySettings(signingKey, issuer, audience, adminKey);
        services.AddSingleton(appSettings);

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

        }).AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = appSettings.Issuer,
                ValidAudience = appSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.SigningKey)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });
        services.AddAuthorization();

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IRegisterValidationService, RegisterValidationService>();
        return services;
    }
}
