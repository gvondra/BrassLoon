using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.WorkTask;
using BrassLoon.Interface.WorkTask.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
