using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BrassLoon.Client.ViewModel
{
    public class DomainUserSearchVM : ViewModelBase
    {
        private readonly DomainVM _domainVM;
        private string _searchText;
        private DomainUserVM _selectedUser;
        private ICommand _search;

        public DomainUserSearchVM(DomainVM domainVM)
        {
            _domainVM = domainVM;
        }

        internal DomainVM DomainVM => _domainVM;

        public ObservableCollection<DomainUserVM> Users { get; } = new ObservableCollection<DomainUserVM>();

        public DomainUserVM SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;
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

        public ICommand Search
        {
            get => _search;
            set
            {
                if (_search != value)
                {
                    _search = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
