using BrassLoon.Client.ViewModel;

namespace BrassLoon.Client.Behaviors
{
    public class DomainClientValidator
    {
        private readonly DomainClientVM _clientVM;

        public DomainClientValidator(DomainClientVM clientVM)
        {
            _clientVM = clientVM;
            clientVM.PropertyChanged += ClientVM_PropertyChanged;
        }

        private void ClientVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_clientVM[e.PropertyName] != null)
                _clientVM[e.PropertyName] = null;
            switch (e.PropertyName)
            {
                case nameof(ClientVM.Name):
                    RequiredTextField(e.PropertyName, _clientVM.Name, _clientVM);
                    break;
                case nameof(ClientVM.Secret):
                    ValiateSecret(_clientVM);
                    break;
            }
        }

        private static void ValiateSecret(DomainClientVM clientVM)
        {
            if (clientVM.IsNew && string.IsNullOrEmpty(clientVM.Secret))
            {
                clientVM[nameof(ClientVM.Secret)] = "Is Required";
            }
        }

        private static void RequiredTextField(string propertyName, string value, ViewModelBase viewModel)
        {
            if (string.IsNullOrEmpty(value))
                viewModel[propertyName] = "Is required";
        }
    }
}
