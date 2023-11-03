using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Account;
using BrassLoon.Interface.Account.Models;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BrassLoon.Client.Behaviors
{
    public class InvitationCancel : ICommand
    {
        private readonly ISettingsFactory _settingsFactory;
        private readonly IUserInvitationService _userInvitationService;
        private bool _canExecute = true;

        public InvitationCancel(ISettingsFactory settingsFactory, IUserInvitationService userInvitationService)
        {
            _settingsFactory = settingsFactory;
            _userInvitationService = userInvitationService;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute;

        public void Execute(object parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));
            if (parameter is UserInvitationVM invitationVM)
            {
                _canExecute = false;
                CanExecuteChanged.Invoke(this, new EventArgs());
                Task.Run(() => Cancel(invitationVM.InnerInvitation))
                    .ContinueWith(CancelCallback, invitationVM, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void Cancel(UserInvitation invitation)
        {
            invitation.Status = -1;
            _userInvitationService.Update(_settingsFactory.CreateAccountSettings(), invitation).Wait();
        }

        private async Task CancelCallback(Task cancel, object state)
        {
            try
            {
                await cancel;
                MessageBox.Show("Invitation Cancelled", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
            }
        }
    }
}
