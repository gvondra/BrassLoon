using System;
using System.Windows.Input;
using Models = BrassLoon.Interface.Account.Models;

namespace BrassLoon.Client.ViewModel
{
    public class ClientVM : ViewModelBase
    {
        private readonly Models.Client _client;
        private string _secret = string.Empty;
        private ICommand _generateSecret;
        private ICommand _save;

        public ClientVM (Models.Client client)
        {
            _client = client;
        }

        internal Models.Client InnerClient => _client;

        public Guid? ClientId
        {
            get => _client.ClientId;
            set
            {
                if (_client.ClientId.HasValue != value.HasValue
                    || (value.HasValue && _client.ClientId.Value != value.Value))
                {
                    _client.ClientId = value;
                    NotifyPropertyChanged();
                }
            }
        }
        

        public string Name
        {
            get => _client.Name ?? string.Empty;
            set
            {
                if (_client.Name != value)
                {
                    _client.Name = value;
                    NotifyPropertyChanged(nameof(Name));
                }
            }
        }

        public bool IsActive
        {
            get => _client.IsActive;
            set
            {
                if (_client.IsActive != value)
                {
                    _client.IsActive = value;
                    NotifyPropertyChanged(nameof(IsActive));
                }
            }
        }

        public string Secret
        {
            get => _secret ?? string.Empty;
            set
            {
                value = value ?? string.Empty;
                if (_secret != value)
                {
                    _secret = value;
                    NotifyPropertyChanged(nameof(Secret));
                }
            }
        }

        public ICommand GenerateSecret
        {
            get => _generateSecret;
            set
            {
                if (_generateSecret != value)
                {
                    _generateSecret = value;
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
