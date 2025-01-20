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
        private readonly IWorkTaskStatusService _workTaskStatusService;

        public WorkTaskTypeTest(
            AppSettings appSettings,
            ISettingsFactory settingsFactory,
            ILogger logger,
            IWorkTaskTypeService workTaskTypeService,
            IWorkTaskStatusService workTaskStatusService)
        {
            _appSettings = appSettings;
            _settingsFactory = settingsFactory;
            _logger = logger;
            _workTaskTypeService = workTaskTypeService;
            _workTaskStatusService = workTaskStatusService;
        }

        public async Task Execute()
        {
            WorkTaskSettings settings = _settingsFactory.CreateWorkTaskSettings();
            _logger.Information("Getting all work task types");
            List<WorkTaskType> workTaskTypes = await _workTaskTypeService.GetAll(settings, _appSettings.Domain.Value);
            WorkTaskType testType = workTaskTypes.Find(wtt => Regex.IsMatch(wtt.Title, @"^TestClient\s*Generated", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(200)));
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
            _logger.Information($"Title returned from update {testType.Title}");
            testType = await _workTaskTypeService.Get(settings, _appSettings.Domain.Value, testType.WorkTaskTypeId.Value);
            _logger.Information($"Title returned from get {testType.Title}");
            testType = await _workTaskTypeService.GetByCode(settings, _appSettings.Domain.Value, testType.Code);
            _logger.Information($"Title returned from get by code {testType.Title}");

            await ExecuteStatus(testType);
            await ExecuteDeleteStatus(testType);
        }

        private async Task ExecuteStatus(WorkTaskType testType)
        {
            WorkTaskSettings settings = _settingsFactory.CreateWorkTaskSettings();
            _logger.Information("Getting work task status");
            List<WorkTaskStatus> workTaskStatuses = testType.Statuses;
            WorkTaskStatus testStatus = workTaskStatuses.Find(wts => Regex.IsMatch(wts.Name, @"^TestClient\s*Generated", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(200)));
            if (testStatus == null)
            {
                testStatus = new WorkTaskStatus
                {
                    Code = "tst-clnt-gen-status",
                    Description = "Create by test client",
                    DomainId = _appSettings.Domain.Value,
                    Name = $"TestCient Generated {DateTime.Now:O}",
                    WorkTaskTypeId = testType.WorkTaskTypeId.Value,
                    IsClosedStatus = false
                };
                testStatus = await _workTaskStatusService.Create(settings, testStatus);
                _logger.Information($"Created work task status {testStatus.Name}");
            }
            else
            {
                _logger.Information($"Found work task status {testStatus.Name}");
            }
            string updatedName = $"TestClient Generated {DateTime.Now:O}";
            _logger.Information($"Changing status name to {updatedName}");
            testStatus.Name = updatedName;
            testStatus = await _workTaskStatusService.Update(settings, testStatus);
            _logger.Information($"Name returned from update {testStatus.Name}");
            testType = await _workTaskTypeService.Get(settings, testType.DomainId.Value, testType.WorkTaskTypeId.Value);
            testStatus = testType.Statuses.FirstOrDefault(sts => sts.DomainId == _appSettings.Domain.Value && sts.WorkTaskStatusId == testStatus.WorkTaskStatusId.Value);
            _logger.Information($"Name returned from get {testStatus.Name}");
        }

        private async Task ExecuteDeleteStatus(WorkTaskType testType)
        {
            WorkTaskSettings settings = _settingsFactory.CreateWorkTaskSettings();
            _logger.Information("Getting work task status for delete test");
            List<WorkTaskStatus> workTaskStatuses = testType.Statuses;
            WorkTaskStatus testStatus = workTaskStatuses.Find(wts => Regex.IsMatch(wts.Name, @"^TestClient\s*To\s*Delete", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(200)));
            if (testStatus == null)
            {
                testStatus = new WorkTaskStatus
                {
                    Code = "tst-clnt-gen-delete",
                    Description = "Create by test client",
                    DomainId = _appSettings.Domain.Value,
                    Name = $"TestClient To Delete {DateTime.Now:O}",
                    WorkTaskTypeId = testType.WorkTaskTypeId.Value,
                    IsClosedStatus = false
                };
                testStatus = await _workTaskStatusService.Create(settings, testStatus);
                _logger.Information($"Created work task status {testStatus.Name}");
            }
            await _workTaskStatusService.Delete(settings, _appSettings.Domain.Value, testStatus.WorkTaskTypeId.Value, testStatus.WorkTaskStatusId.Value);
        }
    }
}
