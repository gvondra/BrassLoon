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
        private readonly IWorkTaskCommentService _workTaskCommentService;

        public WorkTaskTest(
            AppSettings appSettings,
            ISettingsFactory settingsFactory,
            ILogger logger,
            IWorkTaskService workTaskService,
            IWorkTaskTypeService workTaskTypeService,
            IWorkTaskCommentService workTaskCommentService)
        {
            _appSettings = appSettings;
            _settingsFactory = settingsFactory;
            _logger = logger;
            _workTaskService = workTaskService;
            _workTaskTypeService = workTaskTypeService;
            _workTaskCommentService = workTaskCommentService;
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
                testStatus = await GetWorkTaskStatus(settings, testType.Statuses, testType.WorkTaskTypeId.Value);
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
                    WorkTaskType = testType,
                    WorkTaskContexts = new List<WorkTaskContext> { new WorkTaskContext { ReferenceType = 17, ReferenceValue = Guid.NewGuid().ToString("D") } }
                };
                task = await _workTaskService.Create(settings, task);
                _logger.Information($"Created task {task.Title}");
                string updatedTitle = $"TestClient Generated {DateTime.Now:O}";
                _logger.Information($"Changing type title to {updatedTitle}");
                task.Title = updatedTitle;
                task.WorkTaskContexts = new List<WorkTaskContext> { new WorkTaskContext { ReferenceType = 17, ReferenceValue = Guid.NewGuid().ToString("D") } };
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
                await AddCommentsTest(settings, task);
            }
            await foreach (Models.WorkTask task in await _workTaskService.GetAll(settings, _appSettings.Domain.Value))
            {
                if (Regex.IsMatch(task.Title, @"^TestClient\s*Generated", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(200)))
                    _logger.Information($"Found task {task.Title}");
            }
        }

        private async Task AddCommentsTest(WorkTaskSettings settings, Models.WorkTask task)
        {
            Comment[] CreateComments()
            {
                Comment[] comments = new Comment[25];
                for (int i = 0; i < comments.Length; i += 1)
                {
                    comments[i] = new Comment { Text = $"new comment {i + 1}", DomainId = _appSettings.Domain.Value };
                }
                return comments;
            }
            for (int i = 0; i < 25; i += 1)
            {
                _ = await _workTaskCommentService.Create(settings, task.WorkTaskId.Value, CreateComments());
            }
            List<Comment> comments = await _workTaskCommentService.GetAll(settings, _appSettings.Domain.Value, task.WorkTaskId.Value);
            _logger.Information("Retreived {0} comments", comments.Count);
        }

        private async Task<WorkTaskType> GetWorkTaskType(WorkTaskSettings settings)
        {
            List<WorkTaskType> workTaskTypes = await _workTaskTypeService.GetAll(settings, _appSettings.Domain.Value);
            return workTaskTypes.Find(wtt => Regex.IsMatch(wtt.Title, @"^TestClient\s*Generated", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(200)));
        }

        private async Task<WorkTaskStatus> GetWorkTaskStatus(WorkTaskSettings settings, List<WorkTaskStatus> statuses, Guid workTaskTypeId)
        {
            _logger.Information("Getting work task status");
            return statuses.Find(wts => Regex.IsMatch(wts.Name, @"^TestClient\s*Generated", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(200)));
        }
    }
}
