#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace BrassLoon.WorkTask.Purger
{
    public class SettingsFactory : ISettingsFactory
    {
        private readonly AppSettings _appSettings;

        public SettingsFactory(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public CoreSettings CreateCore() => new CoreSettings(_appSettings);
    }
}
#pragma warning restore IDE0130 // Namespace does not match folder structure