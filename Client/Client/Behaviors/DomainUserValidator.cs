using BrassLoon.Client.ViewModel;

namespace BrassLoon.Client.Behaviors
{
    public class DomainUserValidator
    {
        private readonly DomainUserVM _userVM;

        public DomainUserValidator(DomainUserVM userVM)
        {
            _userVM = userVM;
            userVM.PropertyChanged += UserVM_PropertyChanged;
        }

        private void UserVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_userVM[e.PropertyName] != null)
                _userVM[e.PropertyName] = null;
            switch (e.PropertyName)
            {
                case nameof(DomainUserVM.Name):
                    RequiredTextField(e.PropertyName, _userVM.Name, _userVM);
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
