using BrassLoon.CommonAPI;

namespace AccountAPI
{
    public class SettingsFactory
    {
        public CoreSettings CreateCore(CommonApiSettings settings)
        {
            return new CoreSettings(settings);
        }

        public LogSettings CreateLog(CommonApiSettings settings, string accessToken)
        {
            return new LogSettings(accessToken)
            {
                BaseAddress = settings.LogApiBaseAddress
            };
        }
    }
}
