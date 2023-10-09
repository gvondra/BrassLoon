using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Account;
using BrassLoon.Interface.Account.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Client.Behaviors
{
    public class AccountLoader
    {
        private readonly ISettingsFactory _settingsFactory;
        private readonly IAccountService _accountService;
        private readonly IDomainService _domainService;
        private readonly IUserInvitationService _userInvitationService;
        private readonly AccountUserRemover _accountUserRemover;
        private readonly AccountVM _accountVM;

        public AccountLoader(
            ISettingsFactory settingsFactory,
            IAccountService accountService,
            IDomainService domainService,
            IUserInvitationService userInvitationService,
            AccountUserRemover accountUserRemover,
            AccountVM accountVM)
        {
            _settingsFactory = settingsFactory;
            _accountService = accountService;
            _domainService = domainService;
            _userInvitationService = userInvitationService;
            _accountUserRemover = accountUserRemover;
            _accountVM = accountVM;
        }

        public void LoadDomains()
        {
            _accountVM.Domains.Clear();
            Task.Run(() => LoadDomains(_accountVM.AccountId))
                .ContinueWith(LoadDomainsCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private List<Domain> LoadDomains(Guid accountId)
        {
            return _domainService.GetByAccountId(_settingsFactory.CreateAccountSettings(), accountId).Result;
        }

        private async Task LoadDomainsCallback(Task<List<Domain>> loadDomains, object state)
        {
            try
            {
                _accountVM.Domains.Clear();
                foreach (Domain domain in await loadDomains)
                {
                    _accountVM.Domains.Add(new DomainVM(domain));
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
            }
        }

        public void LoadDeletedDomains()
        {
            _accountVM.DeletedDomains.Clear();
            Task.Run(() => LoadDeletedDomains(_accountVM.AccountId))
                .ContinueWith(LoadDeletedDomainsCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private List<Domain> LoadDeletedDomains(Guid accountId)
        {
            return _domainService.GetDeletedByAccountId(_settingsFactory.CreateAccountSettings(), accountId).Result;
        }

        private async Task LoadDeletedDomainsCallback(Task<List<Domain>> loadDomains, object state)
        {
            try
            {
                _accountVM.DeletedDomains.Clear();
                foreach (Domain domain in await loadDomains)
                {
                    _accountVM.DeletedDomains.Add(new DomainVM(domain));
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
            }
        }

        public void LoadUsers()
        {
            _accountVM.Users.Clear();
            Task.Run(() => LoadUsers(_accountVM.AccountId))
                .ContinueWith(LoadUsersCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private List<User> LoadUsers(Guid accountId)
        {
            return _accountService.GetUsers(_settingsFactory.CreateAccountSettings(), accountId).Result;
        }

        private async Task LoadUsersCallback(Task<List<User>> loadUsers, object state)
        {
            try
            {
                _accountVM.Users.Clear();
                foreach (User user in await loadUsers)
                {
                    _accountVM.Users.Add(new UserVM(user));
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
            }
        }

        public void LoadInvitations()
        {
            _accountVM.Invitations.Clear();
            Task.Run(() => LoadInvitations(_accountVM.AccountId))
                .ContinueWith(LoadInvitationsCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private List<UserInvitation> LoadInvitations(Guid accountId)
        {
            return _userInvitationService.GetByAccountId(_settingsFactory.CreateAccountSettings(), accountId).Result;
        }

        private async Task LoadInvitationsCallback(Task<List<UserInvitation>> loadUserInvitations, object state)
        {
            try
            {
                _accountVM.Invitations.Clear();
                foreach (UserInvitation invitation in await loadUserInvitations)
                {
                    _accountVM.Invitations.Add(new UserInvitationVM(invitation));
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
            }
        }
    }
}
