using BrassLoon.CommonAPI;

namespace ConfigAPI
{
    public interface ISettingsFactory
    {
        CoreSettings CreateCore(Settings settings);
        AccountSettings CreateAccount(CommonApiSettings settings, string accessToken);
        LogSettings CreateLog(CommonApiSettings settings, string accessToken);
        LogSettings CreateLog(CommonApiSettings settings);
    }
}
