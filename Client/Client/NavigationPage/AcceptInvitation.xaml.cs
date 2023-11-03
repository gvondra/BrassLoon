using Autofac;
using BrassLoon.Interface.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Models = BrassLoon.Interface.Account.Models;

namespace BrassLoon.Client.NavigationPage
{
    /// <summary>
    /// Interaction logic for AcceptInvitation.xaml
    /// </summary>
    public partial class AcceptInvitation : Page
    {
        public AcceptInvitation()
        {
            InitializeComponent();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Guid id;
                if (!string.IsNullOrEmpty(InvitationIdTextBox.Text))
                {
                    if (Guid.TryParse(InvitationIdTextBox.Text, out id))
                    {
                        Task.Run(() => GetInvitation(id))
                            .ContinueWith(GetInvitationCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                    else
                    {
                        MessageBox.Show("Invalid id", "Id Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }

        private Models.UserInvitation GetInvitation(Guid id)
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifetimeScope();
            ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
            IUserInvitationService userInvitationService = scope.Resolve<IUserInvitationService>();
            try
            {
                return userInvitationService.Get(settingsFactory.CreateAccountSettings(), id).Result;
            }
            catch (System.AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException is RestClient.Exceptions.RequestError innerException)
                {
                    if (innerException.StatusCode == HttpStatusCode.NotFound)
                        return null;
                    else
                        throw;
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task GetInvitationCallback(Task<Models.UserInvitation> getInvitation, object state)
        {
            try
            {
                Models.UserInvitation invitation = await getInvitation;
                if (invitation == null)
                {
                    MessageBox.Show("Invitation not found", "Invitation Status", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (invitation.Status == -1)
                {
                    MessageBox.Show("Invitation Cancelled", "Invitation Status", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (invitation.Status == 0 && invitation.ExpirationTimestamp < DateTime.UtcNow)
                {
                    MessageBox.Show("Invitation Expired", "Invitation Status", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (invitation.Status == 255)
                {
                    _ = Task.Run(() => GetAccount(invitation.AccountId.Value))
                        .ContinueWith(NavigateToAccount, null, TaskScheduler.FromCurrentSynchronizationContext());
                }
                else
                {
                    invitation.Status = 255;
                    _ = Task.Run(() => Save(invitation))
                        .ContinueWith(SaveCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }

        private Models.UserInvitation Save(Models.UserInvitation invitation)
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifetimeScope();
            ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
            IUserInvitationService userInvitationService = scope.Resolve<IUserInvitationService>();
            return userInvitationService.Update(settingsFactory.CreateAccountSettings(), invitation).Result;
        }

        private async Task SaveCallback(Task<Models.UserInvitation> save, object state)
        {
            try
            {
                Models.UserInvitation invitation = await save;
                _ = Task.Run(() => GetAccount(invitation.AccountId.Value))
                    .ContinueWith(NavigateToAccount, null, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }

        private Models.Account GetAccount(Guid accountId)
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifetimeScope();
            ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
            IAccountService accountService = scope.Resolve<IAccountService>();
            return accountService.Get(settingsFactory.CreateAccountSettings(), accountId).Result;
        }

        private async Task NavigateToAccount(Task<Models.Account> getAccount, object state)
        {
            try
            {
                Models.Account account = await getAccount;
                NavigationService navigationService = NavigationService.GetNavigationService(this);
                navigationService.Navigate(
                    new Account(new ViewModel.AccountVM(account)));
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }
    }
}
