using BrassLoon.Config.TestClient.Settings;
using BrassLoon.Interface.Config;
using BrassLoon.Interface.Config.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Config.TestClient
{
    public class LookupTest
    {
        private readonly ILogger _logger;
        private readonly AppSettings _appSettings;
        private readonly ISettingsFactory _settingsFactory;
        private readonly ILookupService _lookupService;

        public LookupTest(
            AppSettings appSettings,
            ISettingsFactory settingsFactory,
            ILogger logger,
            ILookupService lookupService)
        {
            _appSettings = appSettings;
            _settingsFactory = settingsFactory;
            _logger = logger;
            _lookupService = lookupService;
        }

        public async Task Execute()
        {
            try
            {
                _logger.Information("Lookup Test Started");
                ConfigSettings settings = _settingsFactory.CreateConfigSettings();
                string[] codes = (await _lookupService.GetCodes(settings, _appSettings.Domain.Value)).ToArray();
                _logger.Information($"Lookup Codes {string.Join(", ", codes)}");
                string code = "test-client-code";
                Dictionary<string, string> values = new Dictionary<string, string>
            {
                { "key-1", "value-1" }
            };
                await _lookupService.Delete(settings, _appSettings.Domain.Value, code);
                Lookup lookup = await _lookupService.Save(settings, _appSettings.Domain.Value, code, values);
                lookup.Data["current"] = DateTime.Now.ToString("O");
                lookup = await _lookupService.Save(settings, _appSettings.Domain.Value, code, lookup.Data);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }
            finally
            {
                _logger.Information("Lookup Test Ended");
            }
        }
    }
}
