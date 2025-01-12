using Microsoft.Extensions.Configuration;

namespace BrassLoon.Client.Settings
{
    internal static class AppSettingsLoader
    {
        public static AppSettings Load()
            => BindConfiguration(GetConfiguration());

        private static AppSettings BindConfiguration(IConfiguration configuration)
        {
            AppSettings settings = new AppSettings();
            ConfigurationBinder.Bind(configuration, settings);
            return settings;
        }

        private static IConfiguration GetConfiguration()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            _ = builder.AddJsonFile("appsettings.json", false);
#if DEBUG
            _ = builder.AddEnvironmentVariables();
#endif
            return builder.Build();
        }
    }
}
