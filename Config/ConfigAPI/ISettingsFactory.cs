using BrassLoon.CommonAPI;

namespace ConfigAPI
{
    public interface ISettingsFactory
    {
        CoreSettings CreateCore(CommonApiSettings settings);
        AccountSettings CreateAccount(CommonApiSettings settings, string accessToken);
        LogSettings CreateLog(CommonApiSettings settings, string accessToken);
        LogSettings CreateLog(CommonApiSettings settings);
    }
}
