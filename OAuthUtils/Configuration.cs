using Microsoft.Extensions.Configuration;

namespace OAuthUtils
{
    internal static class Configuration
    {
        static Configuration()
        {
            ConfigurationRoot = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        }

        public static IConfigurationRoot ConfigurationRoot { get; private set; }
    }
}
