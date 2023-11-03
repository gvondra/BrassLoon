using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.WorkTask;
using BrassLoon.Interface.WorkTask.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace BrassLoon.Client.Behaviors
{
    public class DomainTaskTypeSaver : ICommand
    {
        private readonly ISettingsFactory _settingsFactory;
        private readonly IWorkTaskTypeService _taskTypeService;
        private bool _canExecute = true;

        public DomainTaskTypeSaver(ISettingsFactory settingsFactory, IWorkTaskTypeService taskTypeService)
        {
            _settingsFactory = settingsFactory;
            _taskTypeService = taskTypeService;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute;

        public void Execute(object parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));
            if (parameter is WorkTaskTypeVM taskTypeVM && !taskTypeVM.HasErrors)
            {
                _canExecute = false;
                CanExecuteChanged.Invoke(this, new EventArgs());
                Task.Run(() =>
                {
                    Func<ISettings, WorkTaskType, Task<WorkTaskType>> save = _taskTypeService.Update;
                    if (!taskTypeVM.WorkTaskTypeId.HasValue)
                        save = _taskTypeService.Create;
                    return save(_settingsFactory.CreateWorkTaskSettings(), taskTypeVM.InnerTaskType).Result;
                })
                    .ContinueWith(SaveCallback, taskTypeVM, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private async Task SaveCallback(Task<WorkTaskType> save, object state)
        {
            try
            {
                WorkTaskType taskType = await save;
                if (state is WorkTaskTypeVM taskTypeVM && !taskTypeVM.WorkTaskTypeId.HasValue)
                {
                    int index = taskTypeVM.TaskTypesVM.Items.IndexOf(taskTypeVM);
                    if (index >= 0)
                    {
                        WorkTaskTypeVM newTaskTypeVM = new WorkTaskTypeVM(taskType, taskTypeVM.TaskTypesVM);
                        taskTypeVM.TaskTypesVM.Items[index] = newTaskTypeVM;
                        taskTypeVM.TaskTypesVM.SelectedTaskType = newTaskTypeVM;
                        foreach (WorkTaskStatusVM taskStatusVM in taskTypeVM.Statuses)
                        {
                            newTaskTypeVM.Statuses.Add(new WorkTaskStatusVM(taskStatusVM.InnerTaskStatus, newTaskTypeVM));
                        }
                        taskTypeVM.TaskTypesVM.NavigationService.Navigated += NavigationService_Navigated;
                        NavigationPage.TaskType page = new NavigationPage.TaskType(newTaskTypeVM);
                        taskTypeVM.TaskTypesVM.NavigationService.Navigate(page);
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

        private void NavigationService_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (sender is Frame frame)
            {
                frame.NavigationService.RemoveBackEntry();
                frame.NavigationService.Navigated -= NavigationService_Navigated;
            }
        }
    }
}
