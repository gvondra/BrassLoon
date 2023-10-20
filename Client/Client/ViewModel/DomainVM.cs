using BrassLoon.Interface.Account.Models;
using System;
using System.Windows.Input;

namespace BrassLoon.Client.ViewModel
{
    public class DomainVM : ViewModelBase
    {
        private readonly Domain _domain;
        private ICommand _save;
        private ICommand _delete;

        public DomainVM(Domain domain)
        {
            _domain = domain;
        }

        internal Domain InnerDomain => _domain;

        public Guid DomainId => _domain.DomainId.Value;

        public string Name
        {
            get => _domain.Name;
            set
            {
                if (_domain.Name != value)
                {
                    _domain.Name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand Save
        {
            get => _save;
            set
            {
                if (_save != value)
                {
                    _save = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand Delete
        {
            get => _delete;
            set
            {
                if (_delete != value)
                {
                    _delete = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
