using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.WorkTask;
using BrassLoon.Interface.WorkTask.Models;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BrassLoon.Client.Behaviors
{
    public class DomainTaskStatusSaver : ICommand
    {
        private readonly ISettingsFactory _settingsFactory;
        private readonly IWorkTaskStatusService _taskStatusService;
        private bool _canExecute = true;

        public DomainTaskStatusSaver(ISettingsFactory settingsFactory, IWorkTaskStatusService taskStatusService)
        {
            _settingsFactory = settingsFactory;
            _taskStatusService = taskStatusService;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute;

        public void Execute(object parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));
            if (parameter is WorkTaskStatusVM taskStatusVM && !taskStatusVM.HasErrors)
            {
                if (!taskStatusVM.WorkTaskTypeId.HasValue && !taskStatusVM.TaskTypeVM.WorkTaskTypeId.HasValue)
                {
                    MessageBox.Show("Save the parent work task type", "No Parent", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
                else
                {
                    _canExecute = false;
                    CanExecuteChanged.Invoke(this, new EventArgs());
                    Task.Run(() =>
                    {
                        Func<ISettings, WorkTaskStatus, Task<WorkTaskStatus>> save = _taskStatusService.Update;
                        if (!taskStatusVM.WorkTaskStatusId.HasValue)
                            save = _taskStatusService.Create;
                        if (!taskStatusVM.WorkTaskTypeId.HasValue)
                            taskStatusVM.InnerTaskStatus.WorkTaskTypeId = taskStatusVM.TaskTypeVM.WorkTaskTypeId.Value;
                        return save(_settingsFactory.CreateWorkTaskSettings(), taskStatusVM.InnerTaskStatus).Result;
                    })
                        .ContinueWith(SaveCallback, taskStatusVM, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        private async Task SaveCallback(Task<WorkTaskStatus> save, object state)
        {
            try
            {
                WorkTaskStatus taskStatus = await save;
                if (state is WorkTaskStatusVM taskStatusVM)
                {
                    if (!taskStatusVM.WorkTaskStatusId.HasValue)
                    {
                        int index = taskStatusVM.TaskTypeVM.Statuses.IndexOf(taskStatusVM);
                        if (index >= 0)
                        {
                            WorkTaskStatusVM newTaskStatusVM = new WorkTaskStatusVM(taskStatus, taskStatusVM.TaskTypeVM);
                            taskStatusVM.TaskTypeVM.Statuses[index] = newTaskStatusVM;
                            taskStatusVM.TaskTypeVM.SelectedTaskStatus = newTaskStatusVM;
                        }
                    }
                    if (taskStatusVM.TaskTypeVM.TaskTypesVM.NavigationService != null)
                    {
                        taskStatusVM.TaskTypeVM.TaskTypesVM.NavigationService.GoBack();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex);
            }
            finally
            {
                if (CanExecuteChanged != null)
                {
                    _canExecute = true;
                    CanExecuteChanged.Invoke(this, new EventArgs());
                }
            }
        }
    }
}
