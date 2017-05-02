using Microsoft.Extensions.Logging;
using Serilog;

namespace OAuthUtils
{
    internal static class Logging
    {
        static Logging()
        {
            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration.ConfigurationRoot)
            .CreateLogger();
        }

        public static ILoggerFactory LoggerFactory { get; } = new LoggerFactory().AddSerilog();
        public static ILogger<T> CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
    }
}
