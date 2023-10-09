using BrassLoon.Client.ViewModel;
using System;
using System.Text.RegularExpressions;

namespace BrassLoon.Client.Behaviors
{
    public class CreateInvitationValidator
    {
        private readonly CreateInvitationVM _createInvitationVM;

        public CreateInvitationValidator(CreateInvitationVM createInvitationVM)
        {
            _createInvitationVM = createInvitationVM;
            createInvitationVM.PropertyChanged += CreateInvitationVM_PropertyChanged;
        }

        private void CreateInvitationVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_createInvitationVM[e.PropertyName] != null)
                _createInvitationVM[e.PropertyName] = null;
            switch (e.PropertyName)
            {
                case nameof(CreateInvitationVM.EmailAddress):
                    RequiredTextField(e.PropertyName, _createInvitationVM.EmailAddress, _createInvitationVM);
                    ValidateEmailAddress(e.PropertyName, _createInvitationVM.EmailAddress, _createInvitationVM);
                    break;
                case nameof(CreateInvitationVM.ExpirationTimestamp):
                    ValidateExpiration(e.PropertyName, _createInvitationVM.ExpirationTimestamp, _createInvitationVM);
                    break;
            }
        }

        private static void ValidateExpiration(string propertyName, DateTime? value, ViewModelBase viewModel)
        {
            if (!value.HasValue)
                viewModel[propertyName] = "Is required";
            else if (value.Value.Date < DateTime.Today)
                viewModel[propertyName] = "Should be a future date";
        }

        private static void ValidateEmailAddress(string propertyName, string value, ViewModelBase viewModel)
        {
            if (!string.IsNullOrEmpty(value) && !Regex.IsMatch(value, @".+?@.+?", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(200)))
                viewModel[propertyName] = "Invalid email address";
        }

        private static void RequiredTextField(string propertyName, string value, ViewModelBase viewModel)
        {
            if (string.IsNullOrEmpty(value))
                viewModel[propertyName] = "Is required";
        }
    }
}
