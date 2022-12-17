using AutoMapper;
using BrassLoon.CommonAPI;
using BrassLoon.Interface.Account;
using BrassLoon.Interface.Log;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace AuthorizationAPI
{
    public abstract class AuthorizationContollerBase : CommonControllerBase
    {
        protected readonly IOptions<Settings> _settings;
        protected readonly SettingsFactory _settingsFactory;
        protected readonly IExceptionService _exceptionService;
        protected readonly IDomainService _domainService;
        private readonly MapperFactory _mapperFactory;
        private BrassLoon.Interface.Log.ISettings _loggSettings;
        private BrassLoon.Interface.Account.ISettings _accountSettings;
        private CoreSettings _coreSettings;

        protected AuthorizationContollerBase(IOptions<Settings> settings, 
            SettingsFactory settingsFactory,
            IExceptionService exceptionService,
            MapperFactory mapperFactory,
            IDomainService domainService)
        {
            _settings = settings;
            _settingsFactory = settingsFactory;
            _exceptionService = exceptionService;
            _mapperFactory = mapperFactory;
            _domainService = domainService;
        }

        [NonAction]
        protected IMapper CreateMapper() => _mapperFactory.Create();

        [NonAction]
        protected BrassLoon.Interface.Log.ISettings CreateLogSettings() => CreateLogSettings(_settings.Value, GetAccessToken());

        [NonAction]
        protected override BrassLoon.Interface.Log.ISettings CreateLogSettings(CommonApiSettings settings, string accessToken)
        {
            if (_loggSettings == null)
                _loggSettings = _settingsFactory.CreateLog(settings, accessToken);
            return _loggSettings;
        }

        [NonAction]
        protected BrassLoon.Interface.Account.ISettings CreateAccountSettings() => CreateAccountSettings(_settings.Value, GetAccessToken());

        [NonAction]
        protected override BrassLoon.Interface.Account.ISettings CreateAccountSettings(CommonApiSettings settings, string accessToken)
        {
            if (_accountSettings == null)
                _accountSettings = _settingsFactory.CreateAccount(settings, accessToken);
            return _accountSettings;
        }

        [NonAction]
        protected CoreSettings CreateCoreSettings()
        {
            if (_coreSettings == null)
                _coreSettings = _settingsFactory.CreateCore(_settings.Value);
            return _coreSettings;
        }

        [NonAction]
        protected Task LogException(Exception ex) => LogException(ex, _exceptionService, _settings.Value);

        [NonAction]
        protected Task<bool> VerifyDomainAccount(Guid domainId) => VerifyDomainAccount(domainId, _settings.Value, _domainService);
    }
}
