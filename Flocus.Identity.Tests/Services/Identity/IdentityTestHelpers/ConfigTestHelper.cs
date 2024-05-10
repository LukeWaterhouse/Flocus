using Microsoft.Extensions.Configuration;
using System.Text;

namespace Flocus.Identity.Tests.Services.Identity.IdentityTestHelpers;

public static class ConfigTestHelper
{
    public static IConfigurationSection GenerateConfigSection(string signingKeyValue, string adminKeyValue)
    {
        var appSettings = $@"{{
            ""AppSettings"":{{
                ""SigningKey"":""{signingKeyValue}"",
                ""AdminKey"":""{adminKeyValue}""
            }}
        }}";

        var builder = new ConfigurationBuilder();
        builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));
        var configuration = builder.Build();

        return configuration.GetSection("AppSettings");
    }
}
