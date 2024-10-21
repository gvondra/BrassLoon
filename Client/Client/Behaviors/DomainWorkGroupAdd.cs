using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.WorkTask.Models;
using System;
using System.Windows.Input;

namespace BrassLoon.Client.Behaviors
{
    public class DomainWorkGroupAdd : ICommand
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
                AddWorkGroup(domainVM.WorkGroups);
            }
            else if (parameter is WorkGroupsVM workGroupsVM)
            {
                AddWorkGroup(workGroupsVM);
            }
        }

        private static void AddWorkGroup(WorkGroupsVM workGroupsVM)
        {
            WorkGroupVM workGroupVM = new WorkGroupVM(
                new WorkGroup { DomainId = workGroupsVM.DomainVM.DomainId },
                workGroupsVM);
            workGroupVM.Title = "New Group";
            workGroupsVM.Items.Add(workGroupVM);
            workGroupsVM.SelectedGroup = workGroupVM;
            if (workGroupsVM.NavigationService != null)
            {
                NavigationPage.WorkGroup page = new NavigationPage.WorkGroup(workGroupVM);
                _ = workGroupsVM.NavigationService.Navigate(page);
            }
        }
    }
}
