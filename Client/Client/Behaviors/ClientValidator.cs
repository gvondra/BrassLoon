using BrassLoon.Client.ViewModel;

namespace BrassLoon.Client.Behaviors
{
    public class ClientValidator
    {
        private readonly ClientVM _clientVM;

        public ClientValidator(ClientVM clientVM)
        {
            _clientVM = clientVM;
            _clientVM.PropertyChanged += ClientVM_PropertyChanged;
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
            }
        }

        private static void RequiredTextField(string propertyName, string value, ViewModelBase viewModel)
        {
            if (string.IsNullOrEmpty(value))
                viewModel[propertyName] = "Is required";
        }
    }
}
