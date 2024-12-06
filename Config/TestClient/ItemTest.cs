using BrassLoon.Config.TestClient.Settings;
using BrassLoon.Interface.Config;
using BrassLoon.Interface.Config.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Config.TestClient
{
    public class ItemTest
    {
        private readonly ILogger _logger;
        private readonly AppSettings _appSettings;
        private readonly ISettingsFactory _settingsFactory;
        private readonly IItemService _itemService;

        public ItemTest(
            AppSettings appSettings,
            ISettingsFactory settingsFactory,
            ILogger logger,
            IItemService itemService)
        {
            _appSettings = appSettings;
            _settingsFactory = settingsFactory;
            _logger = logger;
            _itemService = itemService;
        }

#pragma warning disable S1854 // Unused assignments should be removed
        public async Task Execute()
        {
            try
            {
                _logger.Information("Item Test Started");
                ConfigSettings settings = _settingsFactory.CreateConfigSettings();
                string[] codes = (await _itemService.GetCodes(settings, _appSettings.Domain.Value)).ToArray();
                _logger.Information($"Item Codes {string.Join(", ", codes)}");
                string code = "test-client-code";
                object value = new
                {
                    Key1 = "value-1",
                    Key2 = new
                    {
                        Key3 = "value-3"
                    }
                };
                await _itemService.Delete(settings, _appSettings.Domain.Value, code);
                Item item = await _itemService.Save(settings, _appSettings.Domain.Value, code, value);
                item.Data["current"] = DateTime.Now.ToString("O");
                item = await _itemService.Save(settings, _appSettings.Domain.Value, code, item.Data);
                _ = await _itemService.GetHistoryByCode(settings, _appSettings.Domain.Value, code);
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
#pragma warning restore S1854 // Unused assignments should be removed
    }
}
