using BrassLoon.Client.Settings;
using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.WorkTask;
using BrassLoon.Interface.WorkTask.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Client.Behaviors
{
    public class DomainTaskTypeLoader
    {
        private readonly DomainVM _domainVM;
        private readonly ISettingsFactory _settingsFactory;
        private readonly IWorkTaskTypeService _workTaskTypeService;
        private readonly IWorkTaskStatusService _workTaskStatusService;

        public DomainTaskTypeLoader(
            DomainVM domainVM,
            ISettingsFactory settingsFactory,
            IWorkTaskTypeService workTaskTypeService,
            IWorkTaskStatusService workTaskStatusService)
        {
            _domainVM = domainVM;
            _settingsFactory = settingsFactory;
            _workTaskTypeService = workTaskTypeService;
            _workTaskStatusService = workTaskStatusService;
            domainVM.TaskTypes.Items.CollectionChanged += Item_CollectionChanged;
        }

        public void LoadWorkTaskTypes()
        {
            _domainVM.TaskTypes.IsLoadingTaskTypes = true;
            _domainVM.TaskTypes.Items.Clear();
            Task.Run(() => _workTaskTypeService.GetAll(_settingsFactory.CreateWorkTaskSettings(), _domainVM.DomainId).Result)
                .ContinueWith(LoadWorkTaskTypesCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task LoadWorkTaskTypesCallback(Task<List<WorkTaskType>> loadWorkTaskTypes, object state)
        {
            try
            {
                List<WorkTaskType> taskTypes = await loadWorkTaskTypes;
                _domainVM.TaskTypes.Items.Clear();
                foreach (WorkTaskType taskType in taskTypes)
                {
                    _domainVM.TaskTypes.Items.Add(new WorkTaskTypeVM(taskType, _domainVM.TaskTypes));
                }
                if (_domainVM.TaskTypes.Items.Count > 0)
                    _domainVM.TaskTypes.SelectedTaskType = _domainVM.TaskTypes.Items[0];
                _ = Task.Run(() => LoadTaskStatuses(taskTypes))
                    .ContinueWith(LoadTaskStatusesCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex);
                _domainVM.TaskTypes.IsLoadingTaskTypes = false;
            }
        }

        private static Dictionary<Guid, IEnumerable<WorkTaskStatus>> LoadTaskStatuses(List<WorkTaskType> taskTypes)
        {
            return taskTypes.SelectMany(tt => tt.Statuses)
                .GroupBy(s => s.WorkTaskTypeId.Value)
                .ToDictionary<IGrouping<Guid, WorkTaskStatus>, Guid, IEnumerable<WorkTaskStatus>>(
                    g => g.Key,
                    g => g);
        }

        private async Task LoadTaskStatusesCallback(Task<Dictionary<Guid, IEnumerable<WorkTaskStatus>>> loadTaskStatues, object state)
        {
            try
            {
                Dictionary<Guid, IEnumerable<WorkTaskStatus>> taskStatuses = await loadTaskStatues;
                foreach (WorkTaskTypeVM taskTypeVM in _domainVM.TaskTypes.Items)
                {
                    taskTypeVM.Statuses.Clear();
                    if (taskStatuses.ContainsKey(taskTypeVM.WorkTaskTypeId.Value))
                    {
                        foreach (WorkTaskStatus taskStatus in taskStatuses[taskTypeVM.WorkTaskTypeId.Value])
                        {
                            taskTypeVM.Statuses.Add(new WorkTaskStatusVM(taskStatus, taskTypeVM));
                        }
                    }
                    if (taskTypeVM.Statuses.Count > 0)
                    {
                        taskTypeVM.SelectedTaskStatus = taskTypeVM.Statuses[0];
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex);
            }
            finally
            {
                _domainVM.TaskTypes.IsLoadingTaskTypes = false;
            }
        }

        public void Item_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add
                || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (WorkTaskTypeVM taskTypeVM in e.NewItems)
                {
                    if (!taskTypeVM.ContainsBehavior<DomainTaskTypeValidator>())
                        taskTypeVM.AddBehavior(new DomainTaskTypeValidator(taskTypeVM));
                    if (taskTypeVM.Add == null)
                        taskTypeVM.Add = new DomainTaskStatusAdd();
                    if (taskTypeVM.Save == null)
                        taskTypeVM.Save = new DomainTaskTypeSaver(_settingsFactory, _workTaskTypeService);
                    taskTypeVM.Statuses.CollectionChanged += Statuses_CollectionChanged;
                }
            }
        }

        private void Statuses_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add
                || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (WorkTaskStatusVM taskStatusVM in e.NewItems)
                {
                    if (!taskStatusVM.ContainsBehavior<DomainTaskStatusValidator>())
                        taskStatusVM.AddBehavior(new DomainTaskStatusValidator(taskStatusVM));
                    if (taskStatusVM.Save == null)
                        taskStatusVM.Save = new DomainTaskStatusSaver(_settingsFactory, _workTaskStatusService);
                }
            }
        }
    }
}
