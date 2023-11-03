using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Navigation;

namespace BrassLoon.Client.ViewModel
{
    public class WorkGroupsVM : ViewModelBase
    {
        private readonly DomainVM _domainVM;
        private readonly NavigationService _navigationService;
        private bool _isLoadingGroups;
        private WorkGroupVM _selectedGroup;
        private ICommand _add;

        public WorkGroupsVM(DomainVM domainVM, NavigationService navigationService)
        {
            _domainVM = domainVM;
            _navigationService = navigationService;
        }

        public ObservableCollection<WorkGroupVM> Items { get; } = new ObservableCollection<WorkGroupVM>();

        public DomainVM DomainVM => _domainVM;

        public NavigationService NavigationService => _navigationService;

        public bool IsLoadingGroups
        {
            get => _isLoadingGroups;
            set
            {
                if (_isLoadingGroups != value)
                {
                    _isLoadingGroups = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public WorkGroupVM SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                if (_selectedGroup != value)
                {
                    _selectedGroup = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand Add
        {
            get => _add;
            set
            {
                if (_add != value)
                {
                    _add = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
