using BrassLoon.Interface.Authorization.Models;
using System;

namespace BrassLoon.Client.ViewModel
{
    public class DomainSigningKeyVM : ViewModelBase
    {
        private readonly SigningKey _signingKey;
        private readonly DomainVM _domainVM;

        public DomainSigningKeyVM(SigningKey signingKey, DomainVM domainVM)
        {
            _signingKey = signingKey;
            _domainVM = domainVM;
        }

        internal SigningKey InnerSigningKey => _signingKey;

        internal DomainVM DomainVM => _domainVM;

        public Guid? SigningKeyId => _signingKey.SigningKeyId;

        public Guid? DomainId => _signingKey.DomainId;

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
    }
}
