using BrassLoon.Interface.WorkTask;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Account = BrassLoon.Interface.Account;
namespace BrassLoon.WorkTask.TestClient.Settings
{
    public class WorkTaskSettings : ISettings
    {
        private readonly AppSettings _appSettings;
        private readonly ISettingsFactory _settingsFactory;
        private readonly Account.ITokenService _tokenService;
        private JwtSecurityToken _jwtToken;

        public WorkTaskSettings(AppSettings appSettings,
            ISettingsFactory settingsFactory,
            Account.ITokenService tokenService)
        {
            _appSettings = appSettings;
            _settingsFactory = settingsFactory;
            _tokenService = tokenService;
        }

        public string BaseAddress => _appSettings.WorkTaskApiBaseAddress;

        public async Task<string> GetToken()
        {
            AccountSettings settings = _settingsFactory.CreateAccountSettings();
            return await _tokenService.CreateClientCredentialToken(settings, _appSettings.ClientId.Value, _appSettings.ClientSecret);
        }

        //public async Task<string> GetToken()
        //{
        //    if (_jwtToken == null || _jwtToken.ValidTo < DateTime.UtcNow.AddMinutes(2))
        //    {
        //        AccountSettings settings = _settingsFactory.CreateAccountSettings();
        //        string token = await _tokenService.CreateClientCredentialToken(settings, _appSettings.ClientId.Value, _appSettings.ClientSecret);
        //        _jwtToken = new JwtSecurityToken(token);
        //    }
        //    return _jwtToken.RawData;
        //}
    }
}
