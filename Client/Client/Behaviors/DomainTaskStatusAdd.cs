using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.WorkTask.Models;
using System;
using System.Windows.Input;
using System.Windows.Navigation;

namespace BrassLoon.Client.Behaviors
{
    public class DomainTaskStatusAdd : ICommand
    {
        private bool _canExecute = true;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute;

        public void Execute(object parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));
            if (parameter is WorkTaskTypeVM taskTypeVM)
            {
                AddTaskStatus(taskTypeVM);
            }
        }

        private static void AddTaskStatus(WorkTaskTypeVM taskTypeVM)
        {
            WorkTaskStatusVM taskstatusVM = new WorkTaskStatusVM(
                new WorkTaskStatus { DomainId = taskTypeVM.DomainId, WorkTaskTypeId = taskTypeVM.WorkTaskTypeId },
                taskTypeVM);
            taskstatusVM.Code = "new-status-code";
            taskstatusVM.Name = "New Status";
            taskstatusVM.IsClosedStatus = false;
            taskTypeVM.Statuses.Add(taskstatusVM);
            taskTypeVM.SelectedTaskStatus = taskstatusVM;
            NavigationService navigationService = taskTypeVM.TaskTypesVM?.NavigationService;
            if (navigationService != null)
            {
                NavigationPage.TaskStatus taskStatus = new NavigationPage.TaskStatus(taskstatusVM);
                _ = navigationService.Navigate(taskStatus);
            }
        }
    }
}
