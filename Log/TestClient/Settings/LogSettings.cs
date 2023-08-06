using BrassLoon.Interface.Log;
using System.Threading.Tasks;
using AccountInterface = BrassLoon.Interface.Account;

namespace BrassLoon.Log.TestClient.Settings
{
    public sealed class LogSettings : ISettings
    {
        private readonly AppSettings _appSettings;
        private readonly ISettingsFactory _settingsFactory;
        private readonly AccountInterface.ITokenService _tokenService;

        public LogSettings(AppSettings appSettings, ISettingsFactory settingsFactory, AccountInterface.ITokenService tokenService)
        {
            _appSettings = appSettings;
            _settingsFactory = settingsFactory;
            _tokenService = tokenService;
        }

        public string BaseAddress => _appSettings.LogAPIBaseAddress;

        public Task<string> GetToken()
        {
            AccountSettings settings = _settingsFactory.CreateAccount();
            return _tokenService.CreateClientCredentialToken(settings, _appSettings.ClientId, _appSettings.Secret);
        }
    }
}