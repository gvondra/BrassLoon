using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace BrassLoon.Client.ViewModel
{
    internal sealed class AccountsVM : ViewModelBase
    {
        private string _searchText;
        private ICommand _searchCommand;
        private string _message;
        private Visibility _messageVisibility = Visibility.Collapsed;

        public ObservableCollection<AccountVM> Accounts { get; } = new ObservableCollection<AccountVM>();

        public Visibility MessageVisibility
        {
            get => _messageVisibility;
            set
            {
                if (_messageVisibility != value)
                {
                    _messageVisibility = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                if (_message != value)
                {
                    _message = value;
                    NotifyPropertyChanged();
                    MessageVisibility = !string.IsNullOrEmpty(value) ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        public ICommand SearchCommand
        {
            get => _searchCommand;
            set
            {
                if (_searchCommand != value)
                {
                    _searchCommand = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
