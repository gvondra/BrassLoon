using BrassLoon.Client.ViewModel;

namespace BrassLoon.Client.Behaviors
{
    public class DomainValidator
    {
        private readonly DomainVM _domainVM;

        public DomainValidator(DomainVM domainVM)
        {
            _domainVM = domainVM;
            domainVM.PropertyChanged += DomainVM_PropertyChanged;
        }

        private void DomainVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_domainVM[e.PropertyName] != null)
                _domainVM[e.PropertyName] = null;
            switch (e.PropertyName)
            {
                case nameof(ClientVM.Name):
                    RequiredTextField(e.PropertyName, _domainVM.Name, _domainVM);
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
