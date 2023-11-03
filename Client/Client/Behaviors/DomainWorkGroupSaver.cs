using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.WorkTask;
using BrassLoon.Interface.WorkTask.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BrassLoon.Client.Behaviors
{
    public class DomainWorkGroupSaver : ICommand
    {
        private readonly ISettingsFactory _settingsFactory;
        private readonly IWorkGroupService _workGroupService;
        private bool _canExecute = true;

        public DomainWorkGroupSaver(ISettingsFactory settingsFactory, IWorkGroupService workGroupService)
        {
            _settingsFactory = settingsFactory;
            _workGroupService = workGroupService;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute;

        public void Execute(object parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));
            if (parameter is WorkGroupVM workGroupVM && !workGroupVM.HasErrors)
            {
                _canExecute = false;
                CanExecuteChanged.Invoke(this, new EventArgs());
                Task.Run(() =>
                {
                    Func<ISettings, WorkGroup, Task<WorkGroup>> save = _workGroupService.Update;
                    if (!workGroupVM.WorkGroupId.HasValue)
                        save = _workGroupService.Create;
                    return save(_settingsFactory.CreateWorkTaskSettings(), workGroupVM.InnerWorkGroup).Result;
                })
                    .ContinueWith(SaveCallback, workGroupVM, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private async Task SaveCallback(Task<WorkGroup> save, object state)
        {
            try
            {
                WorkGroup workGroup = await save;
                if (state is WorkGroupVM workGroupVM)
                {
                    if (!workGroupVM.WorkGroupId.HasValue)
                    {
                        WorkGroupVM newWorkGroupVM = new WorkGroupVM(workGroup, workGroupVM.WorkGroupsVM);
                        int index = workGroupVM.WorkGroupsVM.Items.IndexOf(workGroupVM);
                        if (index >= 0)
                        {
                            workGroupVM.WorkGroupsVM.Items[index] = newWorkGroupVM;
                            workGroupVM.WorkGroupsVM.SelectedGroup = newWorkGroupVM;
                        }
                    }
                    if (workGroupVM.WorkGroupsVM.NavigationService != null)
                    {
                        workGroupVM.WorkGroupsVM.NavigationService.GoBack();
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
