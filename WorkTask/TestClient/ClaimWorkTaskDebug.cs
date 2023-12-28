using BrassLoon.Interface.WorkTask;
using BrassLoon.Interface.WorkTask.Models;
using BrassLoon.WorkTask.TestClient.Settings;
using Serilog;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.TestClient
{
    public class ClaimWorkTaskDebug
    {
        private readonly AppSettings _appSettings;
        private readonly ISettingsFactory _settingsFactory;
        private readonly ILogger _logger;
        private readonly IWorkTaskService _workTaskService;

        public ClaimWorkTaskDebug(AppSettings appSettings, ISettingsFactory settingsFactory, ILogger logger, IWorkTaskService workTaskService)
        {
            _appSettings = appSettings;
            _settingsFactory = settingsFactory;
            _logger = logger;
            _workTaskService = workTaskService;
        }
        public async Task Execute()
        {
            WorkTaskSettings settings = _settingsFactory.CreateWorkTaskSettings();
            ClaimWorkTaskResponse response = await _workTaskService.Claim(settings, _appSettings.Domain.Value, Guid.Parse("bc3bd748-f17a-4779-ba10-6484440cd841"), "0fad20cf-3bcd-4472-a4aa-293e7283a1c1");
            _logger.Information(response.Message);
        }
    }
}
