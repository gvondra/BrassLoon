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
        private readonly IWorkTaskTypeService _workTaskTypeService;

        public WorkGroupTest(
            AppSettings appSettings,
            ISettingsFactory settingsFactory,
            ILogger logger,
            IWorkGroupService workGroupService,
            IWorkTaskTypeService workTaskTypeService)
        {
            _appSettings = appSettings;
            _settingsFactory = settingsFactory;
            _logger = logger;
            _workGroupService = workGroupService;
            _workTaskTypeService = workTaskTypeService;
        }

        public async Task Execute()
        {
            WorkTaskSettings settings = _settingsFactory.CreateWorkTaskSettings();
            _logger.Information("Getting work groups");
            List<WorkGroup> workGroups = await _workGroupService.GetAll(settings, _appSettings.Domain.Value);
            WorkGroup testGroup = workGroups.Find(grp => Regex.IsMatch(grp.Title, @"^TestClient\s*Generated", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(200)));
            if (testGroup == null)
            {
                testGroup = new WorkGroup
                {
                    Description = "Created by Test Client",
                    DomainId = _appSettings.Domain.Value,
                    Title = $"TestClient Generated {DateTime.Now:O}",
                    MemberUserIds = new List<string> { Guid.NewGuid().ToString("D") }
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
            testGroup.MemberUserIds = new List<string> { Guid.NewGuid().ToString("D") };
            testGroup = await _workGroupService.Update(settings, testGroup);
            _logger.Information($"Title returned from update {testGroup.Title}");
            testGroup = await _workGroupService.Get(settings, _appSettings.Domain.Value, testGroup.WorkGroupId.Value);
            _logger.Information($"Title returned from get {testGroup.Title}");
            await TaskTypeTests(settings, testGroup);
        }

        private async Task TaskTypeTests(WorkTaskSettings settings, WorkGroup testGroup)
        {
            List<WorkTaskType> taskTypes = await _workTaskTypeService.GetAll(settings, _appSettings.Domain.Value);
            WorkTaskType taskType = taskTypes.FirstOrDefault();
            if (taskType != null)
            {
                _logger.Information("Linking work group {0} to task type {1}", testGroup.Title, taskType.Title);
                await _workGroupService.AddWorkTaskTypeLink(settings, _appSettings.Domain.Value, testGroup.WorkGroupId.Value, taskType.WorkTaskTypeId.Value);
                _logger.Information("Unlinking work group {0} to task type {1}", testGroup.Title, taskType.Title);
                await _workGroupService.DeleteWorkTaskTypeLink(settings, _appSettings.Domain.Value, testGroup.WorkGroupId.Value, taskType.WorkTaskTypeId.Value);
            }
        }
    }
}
