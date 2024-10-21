using Autofac;
using BrassLoon.Interface.Account;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
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
                        _ = Task.Run(() => GetInvitation(id))
                            .ContinueWith(GetInvitationCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                    else
                    {
                        _ = MessageBox.Show("Invalid id", "Id Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }

        private static Models.UserInvitation GetInvitation(Guid id)
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifetimeScope();
            ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
            IUserInvitationService userInvitationService = scope.Resolve<IUserInvitationService>();
            try
            {
                return userInvitationService.Get(settingsFactory.CreateAccountSettings(), id).Result;
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException is RestClient.Exceptions.RequestError innerException)
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
                    _ = MessageBox.Show("Invitation not found", "Invitation Status", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (invitation.Status == -1)
                {
                    _ = MessageBox.Show("Invitation Cancelled", "Invitation Status", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (invitation.Status == 0 && invitation.ExpirationTimestamp < DateTime.UtcNow)
                {
                    _ = MessageBox.Show("Invitation Expired", "Invitation Status", MessageBoxButton.OK, MessageBoxImage.Information);
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
            catch (Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }

        private static Models.UserInvitation Save(Models.UserInvitation invitation)
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
            catch (Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }

        private static Models.Account GetAccount(Guid accountId)
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
                _ = navigationService.Navigate(
                    new Account(new ViewModel.AccountVM(account)));
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }
    }
}
