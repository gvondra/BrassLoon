using BrassLoon.Interface.Account.Models;

namespace BrassLoon.Client.ViewModel
{
    internal class AccountVM
    {
        private readonly Account _account;

        public AccountVM(Account account)
        {
            _account = account;
        }

        public string Name => _account.Name;
    }
}
