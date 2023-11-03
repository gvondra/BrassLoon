using BrassLoon.Interface.Authorization.Models;
using System;
using System.Windows.Input;

namespace BrassLoon.Client.ViewModel
{
    public class DomainSigningKeyVM : ViewModelBase
    {
        private readonly SigningKey _signingKey;
        private readonly DomainVM _domainVM;
        private ICommand _save;

        public DomainSigningKeyVM(SigningKey signingKey, DomainVM domainVM)
        {
            _signingKey = signingKey;
            _domainVM = domainVM;
        }

        internal SigningKey InnerSigningKey => _signingKey;

        internal DomainVM DomainVM => _domainVM;

        public Guid? SigningKeyId => _signingKey.SigningKeyId;

        public Guid? DomainId => _signingKey.DomainId;

        public DateTime? CreateTimestamp => _signingKey.CreateTimestamp?.ToLocalTime() ?? DateTime.Now;

        public bool? IsActive
        {
            get => _signingKey.IsActive;
            set
            {
                if (_signingKey.IsActive != value)
                {
                    _signingKey.IsActive = value;
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
    }
}
