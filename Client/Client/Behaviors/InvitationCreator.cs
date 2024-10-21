using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Account;
using BrassLoon.Interface.Account.Models;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BrassLoon.Client.Behaviors
{
    public class InvitationCreator : ICommand
    {
        private readonly ISettingsFactory _settingsFactory;
        private readonly IUserInvitationService _userInvitationService;
        private bool _canExecute = true;

        public InvitationCreator(ISettingsFactory settingsFactory, IUserInvitationService userInvitationService)
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
            if (parameter is CreateInvitationVM invitationVM && !invitationVM.HasErrors && !string.IsNullOrEmpty(invitationVM.EmailAddress))
            {
                _canExecute = false;
                CanExecuteChanged.Invoke(this, new EventArgs());
                _ = Task.Run(() => Create(invitationVM.AccountId, invitationVM.InnerInvitation))
                    .ContinueWith(CreateCallback, invitationVM, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void Create(Guid accountId, UserInvitation userInvitation)
        => _userInvitationService.Create(_settingsFactory.CreateAccountSettings(), accountId, userInvitation).Wait();

        private static async Task CreateCallback(Task create, object state)
        {
            try
            {
                await create;
                if (state is CreateInvitationVM invitationVM)
                {
                    invitationVM.NextInstructionVisibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex);
            }
        }
    }
}
