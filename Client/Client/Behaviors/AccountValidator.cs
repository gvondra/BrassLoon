using BrassLoon.Client.ViewModel;

namespace BrassLoon.Client.Behaviors
{
    internal class AccountValidator
    {
        private readonly AccountVM _accountVM;

        public AccountValidator(AccountVM accountVM)
        {
            _accountVM = accountVM;
            accountVM.PropertyChanged += AccountVM_PropertyChanged;
        }

        private void AccountVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_accountVM[e.PropertyName] != null)
                _accountVM[e.PropertyName] = null;
            switch (e.PropertyName)
            {
                case nameof(AccountVM.Name):
                    RequiredTextField(e.PropertyName, _accountVM.Name, _accountVM);
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
