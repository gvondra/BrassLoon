using BrassLoon.Interface.WorkTask;
using BrassLoon.Interface.WorkTask.Models;
using BrassLoon.WorkTask.TestClient.Settings;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorkTaskModels = BrassLoon.Interface.WorkTask.Models;

namespace BrassLoon.WorkTask.TestClient
{
    public class WorkTaskPerformanceTest
    {
        private const string _workTaskTypeTitle = "work task performance test";
        private const string _workTaskStatusTitle = "default status";
        private readonly AppSettings _appSettings;
        private readonly ISettingsFactory _settingsFactory;
        private readonly ILogger _logger;
        private readonly IWorkTaskTypeService _workTaskTypeService;
        private readonly IWorkTaskStatusService _workTaskStatusService;
        private readonly IWorkTaskService _workTaskService;
        private readonly object _lock = new { };
        private WorkTaskType _workTaskType;
        private WorkTaskStatus _workTaskStatus;
        private int _workTaskCount;
        private double _workTaskDuration;

        public WorkTaskPerformanceTest(
            AppSettings appSettings,
            ISettingsFactory settingsFactory,
            ILogger logger,
            IWorkTaskTypeService workTaskTypeService,
            IWorkTaskStatusService workTaskStatusService,
            IWorkTaskService workTaskService)
        {
            _appSettings = appSettings;
            _settingsFactory = settingsFactory;
            _logger = logger;
            _workTaskTypeService = workTaskTypeService;
            _workTaskStatusService = workTaskStatusService;
            _workTaskService = workTaskService;
        }

        public async Task Execute()
        {
            _logger.Information("Starting  test execution");
            try
            {
                await InitializeWorkTaskType();
                await GenerateTasks();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }
            _logger.Information("Completed test execution");
        }

        private async Task GenerateTasks()
        {
            WorkTaskSettings settings = _settingsFactory.CreateWorkTaskSettings();
            Queue<Task> tasks = new Queue<Task>();
            foreach (int i in Enumerable.Range(0, 10000))
            {
                while (tasks.Count >= 4)
                {
                    try
                    {
                        await tasks.Dequeue();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, ex.Message);
                    }
                }
                if (i > 0 && i % 100 == 0)
                    _logger.Information($"Created {i:###,##0} work tasks");
                tasks.Enqueue(Task.Run(() => GenerateTask(settings)));
            }
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }
            _logger.Information($"{_workTaskCount} tasks created");
            _logger.Information($"Average create duration {Math.Round(_workTaskDuration / _workTaskCount, 3):###,#00.000} seconds");
        }

        private async Task GenerateTask(WorkTaskSettings settings)
        {
            DateTime start = DateTime.UtcNow;
            WorkTaskModels.WorkTask workTask = new WorkTaskModels.WorkTask
            {
                DomainId = _appSettings.Domain.Value,
                Text = "Test task created",
                Title = "Testing task creation",
                WorkTaskType = _workTaskType,
                WorkTaskStatus = _workTaskStatus,
                WorkTaskContexts = new List<WorkTaskContext>
                {
                    new WorkTaskContext
                    {
                        DomainId = _appSettings.Domain.Value,
                        ReferenceType = 0,
                        ReferenceValue = DateTime.UtcNow.ToString("O")
                    }
                }
            };
            _ = await _workTaskService.Create(settings, workTask);
            double duration = DateTime.UtcNow.Subtract(start).TotalSeconds;
            Monitor.Enter(_lock);
            try
            {
                _workTaskCount += 1;
                _workTaskDuration += duration;
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }

        private async Task InitializeWorkTaskType()
        {
            WorkTaskSettings settings = _settingsFactory.CreateWorkTaskSettings();
            List<WorkTaskType> workTaskTypes = (await _workTaskTypeService.GetAll(settings, _appSettings.Domain.Value)) ?? new List<WorkTaskType>();
            _logger.Information($"Found {workTaskTypes.Count} total work task types");
            _workTaskType = workTaskTypes.Find(wtt => string.Equals(wtt.Title, _workTaskTypeTitle, StringComparison.OrdinalIgnoreCase));
            if (_workTaskType == null)
            {
                _logger.Information($"Creating work task type \"{_workTaskTypeTitle}\"");
                _workTaskType = new WorkTaskType
                {
                    Title = _workTaskTypeTitle,
                    Code = _workTaskTypeTitle.Replace(' ', '-'),
                    Description = "Created for testing purposes",
                    DomainId = _appSettings.Domain.Value
                };
                _workTaskType = await _workTaskTypeService.Create(settings, _workTaskType);
            }
            else
            {
                _logger.Information($"Found existing work task type \"{_workTaskType.Title}\"");
            }
            List<WorkTaskStatus> workTaskStatuses = _workTaskType.Statuses;
            _logger.Information($"Found {workTaskStatuses.Count} total work task statuses");
            _workTaskStatus = workTaskStatuses.Find(wts => string.Equals(wts.Name, _workTaskStatusTitle, StringComparison.OrdinalIgnoreCase));
            if (_workTaskStatus == null)
            {
                _logger.Information($"Creating work task status \"{_workTaskStatusTitle}\"");
                _workTaskStatus = new WorkTaskStatus
                {
                    Name = _workTaskStatusTitle,
                    Code = _workTaskStatusTitle.Replace(' ', '-'),
                    DomainId = _appSettings.Domain.Value,
                    WorkTaskTypeId = _workTaskType.WorkTaskTypeId.Value,
                    IsClosedStatus = false
                };
                _workTaskStatus = await _workTaskStatusService.Create(settings, _workTaskStatus);
            }
            else
            {
                _logger.Information($"Found existing work task status \"{_workTaskStatus.Name}\"");
            }
        }
    }
}
