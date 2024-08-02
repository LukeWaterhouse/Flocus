using Flocus.Identity.Interfaces;
using Flocus.Identity.Interfaces.AdminKeyInterfaces;
using Flocus.Identity.Interfaces.AuthTokenInterfaces;
using Flocus.Identity.Interfaces.PasswordValidationServices;
using Flocus.Identity.Interfaces.RegisterInterfaces;
using Flocus.Identity.Interfaces.RegisterValidationInterfaces;
using Flocus.Identity.Interfaces.RemoveAccountInterfaces;
using Flocus.Identity.Models;
using Flocus.Identity.Services.AdminKeyServices;
using Flocus.Identity.Services.AuthTokenServices;
using Flocus.Identity.Services.ClaimsServices;
using Flocus.Identity.Services.PasswordValidationServices;
using Flocus.Identity.Services.RegisterValidation;
using Flocus.Identity.Services.RegisterValidation.Handlers;
using Flocus.Identity.Services.RegistrationServices;
using Flocus.Identity.Services.RegistrationServices.RegistrationValidationServices.Factories;
using Flocus.Identity.Services.RegistrationServices.RegistrationValidationServices.Handlers;
using Flocus.Identity.Services.RemoveAccountServices;
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

        services.AddScoped<IRemoveAccountService, RemoveAccountService>();
        services.AddScoped<IClaimsService, ClaimsService>();
        services.AddScoped<IAdminKeyService, AdminKeyService>();
        services.AddScoped<IRegistrationService, RegistrationService>();
        services.AddScoped<IAuthTokenService, AuthTokenService>();
        services.AddScoped<IPasswordValidationService, PasswordValidationService>();

        #region Input validation handling
        services.AddScoped<IRegistrationValidationService, RegistrationValidationService>();
        services.AddScoped<IRegistrationValidationChainFactory, RegistrationValidationChainFactory>();
        services.AddScoped<EmailValidationHandler>();
        services.AddScoped<IsAdminValidationHandler>();
        services.AddScoped<PasswordValidationHandler>();
        services.AddScoped<UsernameValidationHandler>();
        #endregion

        return services;
    }
}
