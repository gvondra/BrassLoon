using BrassLoon.Interface.WorkTask;
using BrassLoon.Interface.WorkTask.Models;
using BrassLoon.WorkTask.TestClient.Settings;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Models = BrassLoon.Interface.WorkTask.Models;

namespace BrassLoon.WorkTask.TestClient
{
    public class WorkTaskTest
    {
        private readonly AppSettings _appSettings;
        private readonly ISettingsFactory _settingsFactory;
        private readonly ILogger _logger;
        private readonly IWorkTaskService _workTaskService;
        private readonly IWorkTaskTypeService _workTaskTypeService;
        private readonly IWorkTaskStatusService _workTaskStatusService;

        public WorkTaskTest(
            AppSettings appSettings,
            ISettingsFactory settingsFactory,
            ILogger logger,
            IWorkTaskService workTaskService,
            IWorkTaskTypeService workTaskTypeService,
            IWorkTaskStatusService workTaskStatusService)
        {
            _appSettings = appSettings;
            _settingsFactory = settingsFactory;
            _logger = logger;
            _workTaskService = workTaskService;
            _workTaskTypeService = workTaskTypeService;
            _workTaskStatusService = workTaskStatusService;
        }

        public async Task Execute()
        {
            WorkTaskSettings settings = _settingsFactory.CreateWorkTaskSettings();
            WorkTaskType testType = await GetWorkTaskType(settings);
            WorkTaskStatus testStatus = null;
            if (testType == null)
            {
                _logger.Error("Work task type not found. Run the work task type tests to create it.");
            }
            else
            {
                testStatus = await GetWorkTaskStatus(settings, testType.WorkTaskTypeId.Value);
                if (testStatus == null)
                    _logger.Error("Work task status notfound. Run the work task type tests to create it.");
            }
            if (testType != null && testStatus != null)
            {
                Models.WorkTask task = new Models.WorkTask
                {
                    DomainId = _appSettings.Domain.Value,
                    Text = "Created by test client",
                    Title = $"TestClient Generated {DateTime.Now:O}",
                    WorkTaskStatus = testStatus,
                    WorkTaskType = testType
                };
                task = await _workTaskService.Create(settings, task);
                _logger.Information($"Created task {task.Title}");
                string updatedTitle = $"TestClient Generated {DateTime.Now:O}";
                _logger.Information($"Changing type title to {updatedTitle}");
                task.Title = updatedTitle;
                task = await _workTaskService.Update(settings, task);
                _logger.Information($"Title returned from update {task.Title}");
                task = await _workTaskService.Get(settings, _appSettings.Domain.Value, task.WorkTaskId.Value);
                _logger.Information($"Title returned from get {task.Title}");
                _ = await _workTaskService.Patch(
                    settings,
                    _appSettings.Domain.Value,
                    new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object>
                        {
                            { "WorkTaskId", task.WorkTaskId.Value.ToString("D") },
                            { "WorkTaskStatusId", testStatus.WorkTaskStatusId.Value.ToString("D") }
                        }
                    });
            }
            await foreach (Models.WorkTask task in await _workTaskService.GetAll(settings, _appSettings.Domain.Value))
            {
                if (Regex.IsMatch(task.Title, @"^TestClient\s*Generated", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(200)))
                    _logger.Information($"Found task {task.Title}");
            }
        }

        private async Task<WorkTaskType> GetWorkTaskType(WorkTaskSettings settings)
        {
            List<WorkTaskType> workTaskTypes = await _workTaskTypeService.GetAll(settings, _appSettings.Domain.Value);
            return workTaskTypes.Find(wtt => Regex.IsMatch(wtt.Title, @"^TestClient\s*Generated", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(200)));
        }

        private async Task<WorkTaskStatus> GetWorkTaskStatus(WorkTaskSettings settings, Guid workTaskTypeId)
        {
            _logger.Information("Getting work task status");
            List<WorkTaskStatus> workTaskStatuses = await _workTaskStatusService.GetAll(settings, _appSettings.Domain.Value, workTaskTypeId);
            return workTaskStatuses.Find(wts => Regex.IsMatch(wts.Name, @"^TestClient\s*Generated", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(200)));
        }
    }
}
