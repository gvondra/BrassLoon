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
    public class WorkGroupTest
    {
        private readonly AppSettings _appSettings;
        private readonly ISettingsFactory _settingsFactory;
        private readonly ILogger _logger;
        private readonly IWorkGroupService _workGroupService;

        public WorkGroupTest(
            AppSettings appSettings,
            ISettingsFactory settingsFactory,
            ILogger logger,
            IWorkGroupService workGroupService)
        {
            _appSettings = appSettings;
            _settingsFactory = settingsFactory;
            _logger = logger;
            _workGroupService = workGroupService;
        }

        public async Task Execute()
        {
            WorkTaskSettings settings = _settingsFactory.CreateWorkTaskSettings();
            _logger.Information("Getting work groups");
            List<WorkGroup> workGroups = await _workGroupService.GetAll(settings, _appSettings.Domain.Value);
            WorkGroup testGroup = workGroups.FirstOrDefault(grp => Regex.IsMatch(grp.Title, @"^TestClient\s*Generated", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(200)));
            if (testGroup == null)
            {
                testGroup = new WorkGroup
                {
                    Description = "Created by Test Client",
                    DomainId = _appSettings.Domain.Value,
                    Title = $"TestClient Generated {DateTime.Now:O}"
                };
                testGroup = await _workGroupService.Create(settings, testGroup);
                _logger.Information($"Created group {testGroup.Title}");
            }
            else
            {
                _logger.Information($"Found group {testGroup.Title}");
            }
            string updatedTitle = $"TestClient Generated {DateTime.Now:O}";
            _logger.Information($"Changing type title to {updatedTitle}");
            testGroup.Title = updatedTitle;
            testGroup = await _workGroupService.Update(settings, testGroup);
            _logger.Information($"Title returned from update {testGroup.Title}");
            testGroup = await _workGroupService.Get(settings, _appSettings.Domain.Value, testGroup.WorkGroupId.Value);
            _logger.Information($"Title returned from get {testGroup.Title}");
        }
    }
}
