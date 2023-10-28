using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.WorkTask.Models;
using System;
using System.Windows.Input;

namespace BrassLoon.Client.Behaviors
{
    public class DomainTaskTypeAdd : ICommand
    {
        private bool _canExecute = true;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute;

        public void Execute(object parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));
            if (parameter is DomainVM domainVM && domainVM.TaskTypes != null)
            {
                AddTaskType(domainVM.TaskTypes);
            }
            else if (parameter is WorkTaskTypesVM taskTypesVM)
            {
                AddTaskType(taskTypesVM);
            }
        }

        private void AddTaskType(WorkTaskTypesVM taskTypesVM)
        {
            WorkTaskTypeVM taskTypeVM = new WorkTaskTypeVM(
                new WorkTaskType { DomainId = taskTypesVM.DomainVM.DomainId },
                taskTypesVM);
            taskTypeVM.Code = "new-task-code";
            taskTypeVM.Title = "New Task";
            taskTypesVM.Items.Add(taskTypeVM);
            taskTypesVM.SelectedTaskType = taskTypeVM;
            if (taskTypesVM.NavigationService != null)
            {
                NavigationPage.TaskType taskType = new NavigationPage.TaskType();
                taskTypesVM.NavigationService.Navigate(taskType);
            }
        }
    }
}
