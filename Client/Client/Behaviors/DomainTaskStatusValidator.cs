using BrassLoon.Client.ViewModel;

namespace BrassLoon.Client.Behaviors
{
    public class DomainTaskStatusValidator
    {
        private readonly WorkTaskStatusVM _taskStatusVM;

        public DomainTaskStatusValidator(WorkTaskStatusVM taskStatusVM)
        {
            _taskStatusVM = taskStatusVM;
            taskStatusVM.PropertyChanged += TaskStatusVM_PropertyChanged;
        }

        private void TaskStatusVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_taskStatusVM[e.PropertyName] != null)
                _taskStatusVM[e.PropertyName] = null;
            switch (e.PropertyName)
            {
                case nameof(WorkTaskStatusVM.Code):
                    RequiredTextField(e.PropertyName, _taskStatusVM.Code, _taskStatusVM);
                    break;
                case nameof(WorkTaskStatusVM.Name):
                    RequiredTextField(e.PropertyName, _taskStatusVM.Name, _taskStatusVM);
                    break;
            }
        }

        private static void RequiredTextField(string propertyName, string value, ViewModelBase viewModel)
        {
            if (string.IsNullOrEmpty(value))
                viewModel[propertyName] = "Is required";
        }
    }
}
