using BrassLoon.Interface.Account.Models;
using System;

namespace BrassLoon.Client.ViewModel
{
    public class DomainVM : ViewModelBase
    {
        private readonly Domain _domain;

        public DomainVM(Domain domain)
        {
            _domain = domain;
        }

        public Guid DomainId => _domain.DomainId.Value;
        public string Name => _domain.Name; 
    }
}
