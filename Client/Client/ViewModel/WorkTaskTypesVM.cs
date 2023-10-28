using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Navigation;

namespace BrassLoon.Client.ViewModel
{
    public class WorkTaskTypesVM : ViewModelBase
    {
        private readonly DomainVM _domainVM;
        private readonly NavigationService _navigationService;
        private bool _isLoadingTaskTypes;
        private WorkTaskTypeVM _selectedTaskType;
        private ICommand _add;

        public WorkTaskTypesVM(DomainVM domainVM)
            : this(domainVM, null)
        { }

        public WorkTaskTypesVM(DomainVM domainVM, NavigationService navigationService)
        {
            _domainVM = domainVM;
            _navigationService = navigationService;
        }

        internal DomainVM DomainVM => _domainVM;

        public ObservableCollection<WorkTaskTypeVM> Items { get; } = new ObservableCollection<WorkTaskTypeVM>();

        public NavigationService NavigationService => _navigationService;

        public bool IsLoadingTaskTypes
        {
            get => _isLoadingTaskTypes;
            set
            {
                if (_isLoadingTaskTypes != value)
                {
                    _isLoadingTaskTypes = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public WorkTaskTypeVM SelectedTaskType
        {
            get => _selectedTaskType;
            set
            {
                if (_selectedTaskType != value)
                {
                    _selectedTaskType = value;
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
