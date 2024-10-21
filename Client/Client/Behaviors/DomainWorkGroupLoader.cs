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
    public class DomainWorkGroupLoader
    {
        private readonly DomainVM _domainVM;
        private readonly ISettingsFactory _settingsFactory;
        private readonly IWorkGroupService _workGroupService;
        private readonly IWorkTaskTypeService _workTaskTypeService;

        public DomainWorkGroupLoader(DomainVM domainVM, ISettingsFactory settingsFactory, IWorkGroupService workGroupService, IWorkTaskTypeService workTaskTypeService)
        {
            _domainVM = domainVM;
            _settingsFactory = settingsFactory;
            _workGroupService = workGroupService;
            _workTaskTypeService = workTaskTypeService;
            domainVM.WorkGroups.Items.CollectionChanged += Items_CollectionChanged;
        }

        public void LoadWorkGroups()
        {
            _domainVM.WorkGroups.IsLoadingGroups = true;
            _domainVM.WorkGroups.Items.Clear();
            _ = Task.Run(() => _workGroupService.GetAll(_settingsFactory.CreateWorkTaskSettings(), _domainVM.DomainId).Result)
                .ContinueWith(LoadWorkGroupsCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task LoadWorkGroupsCallback(Task<List<WorkGroup>> loadWorkGroups, object state)
        {
            try
            {
                List<WorkGroup> workGroups = await loadWorkGroups;
                _domainVM.WorkGroups.Items.Clear();
                foreach (WorkGroup workGroup in workGroups)
                {
                    _domainVM.WorkGroups.Items.Add(new WorkGroupVM(workGroup, _domainVM.WorkGroups));
                }
                if (_domainVM.WorkGroups.Items.Count > 0)
                {
                    _domainVM.WorkGroups.SelectedGroup = _domainVM.WorkGroups.Items[0];
                }
                _ = Task.Run(() => _workTaskTypeService.GetAll(_settingsFactory.CreateWorkTaskSettings(), _domainVM.DomainId).Result.ToDictionary(t => t.WorkTaskTypeId.Value))
                    .ContinueWith(GetTaskTypesCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex);
                _domainVM.WorkGroups.IsLoadingGroups = false;
            }
        }

        private async Task GetTaskTypesCallback(Task<Dictionary<Guid, WorkTaskType>> getTaskTypes, object state)
        {
            try
            {
                Dictionary<Guid, WorkTaskType> taskTypes = await getTaskTypes;
                foreach (WorkGroupVM workGroupVM in _domainVM.WorkGroups.Items)
                {
                    foreach (Guid id in workGroupVM.InnerWorkGroup.WorkTaskTypeIds ?? Enumerable.Empty<Guid>())
                    {
                        if (taskTypes.ContainsKey(id))
                            workGroupVM.TaskTypeTitles.Add(taskTypes[id].Title);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex);
            }
            finally
            {
                _domainVM.WorkGroups.IsLoadingGroups = false;
            }
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add
                || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (WorkGroupVM workGroupVM in e.NewItems)
                {
                    if (!workGroupVM.ContainsBehavior<DomainWorkGroupValidator>())
                        workGroupVM.AddBehavior(new DomainWorkGroupValidator(workGroupVM));
                    if (workGroupVM.Save == null)
                        workGroupVM.Save = new DomainWorkGroupSaver(_settingsFactory, _workGroupService);
                }
            }
        }
    }
}
