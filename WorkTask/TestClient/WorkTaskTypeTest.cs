using BrassLoon.Interface.WorkTask;
using BrassLoon.Interface.WorkTask.Models;
using BrassLoon.WorkTask.TestClient.Settings;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.TestClient
{
    public class WorkTaskTypeTest
    {
        private readonly AppSettings _appSettings;
        private readonly ISettingsFactory _settingsFactory;
        private readonly ILogger _logger;
        private readonly IWorkTaskTypeService _workTaskTypeService;

        public WorkTaskTypeTest(
            AppSettings appSettings,
            ISettingsFactory settingsFactory,
            ILogger logger,
            IWorkTaskTypeService workTaskTypeService)
        {
            _appSettings = appSettings;
            _settingsFactory = settingsFactory;
            _logger = logger;
            _workTaskTypeService = workTaskTypeService;
        }

        public async Task Execute()
        {
            WorkTaskSettings settings = _settingsFactory.CreateWorkTaskSettings();
            _logger.Information("Getting all work task types");
            List<WorkTaskType> workTaskTypes = await _workTaskTypeService.GetAll(settings, _appSettings.Domain.Value);
            WorkTaskType testType = workTaskTypes.FirstOrDefault(wtt => Regex.IsMatch(wtt.Title, @"^TestClient\s*Generated", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(200)));
            if (testType == null)
            {
                testType = new WorkTaskType
                {
                    Code = "tst-clnt-gen",
                    Description = "Created by test client",
                    DomainId = _appSettings.Domain.Value,
                    PurgePeriod = 30,
                    Title = $"TestClient Generated {DateTime.Now:O}"
                };
                testType = await _workTaskTypeService.Create(settings, testType);
                _logger.Information($"Created work task type {testType.Title}");
            }
            else
            {
                _logger.Information($"Found work task type {testType.Title}");
            }
            string updatedTitle = $"TestClient Generated {DateTime.Now:O}";
            _logger.Information($"Changing type title to {updatedTitle}");
            testType.Title = updatedTitle;
            testType = await _workTaskTypeService.Update(settings, testType);
            _logger.Information($"Title returned from upate {testType.Title}");
            testType = await _workTaskTypeService.Get(settings, _appSettings.Domain.Value, testType.WorkTaskTypeId.Value);
            _logger.Information($"Title returned from get {testType.Title}");
            testType = await _workTaskTypeService.GetByCode(settings, _appSettings.Domain.Value, testType.Code);
            _logger.Information($"Title returned from get by code {testType.Title}");
        }
    }
}
