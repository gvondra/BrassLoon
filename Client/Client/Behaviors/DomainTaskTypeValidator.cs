using BrassLoon.Client.ViewModel;

namespace BrassLoon.Client.Behaviors
{
    public class DomainTaskTypeValidator
    {
        private readonly WorkTaskTypeVM _taskTypeVM;

        public DomainTaskTypeValidator(WorkTaskTypeVM taskTypeVM)
        {
            _taskTypeVM = taskTypeVM;
            taskTypeVM.PropertyChanged += TaskTypeVM_PropertyChanged;
        }

        private void TaskTypeVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_taskTypeVM[e.PropertyName] != null)
                _taskTypeVM[e.PropertyName] = null;
            switch (e.PropertyName)
            {
                case nameof(WorkTaskTypeVM.Code):
                    RequiredTextField(e.PropertyName, _taskTypeVM.Code, _taskTypeVM);
                    break;
                case nameof(WorkTaskTypeVM.Title):
                    RequiredTextField(e.PropertyName, _taskTypeVM.Title, _taskTypeVM);
                    break;
                case nameof(WorkTaskTypeVM.PurgePeriod):
                    PurgePeriod(_taskTypeVM);
                    break;
            }
        }

        private static void PurgePeriod(WorkTaskTypeVM taskTypeVM)
        {
            if (taskTypeVM.PurgePeriod < 0)
                taskTypeVM[nameof(WorkTaskTypeVM.PurgePeriod)] = "Cannot be negative";
        }

        private static void RequiredTextField(string propertyName, string value, ViewModelBase viewModel)
        {
            if (string.IsNullOrEmpty(value))
                viewModel[propertyName] = "Is required";
        }
    }
}
