using System.Windows;

namespace BrassLoon.Client.ViewModel
{
    internal class HomeVM : ViewModelBase
    {
        private Visibility _systemAdminVisibility = Visibility.Collapsed;
        private Visibility _accountAdminVisibility = Visibility.Collapsed;

        public Visibility SystemAdminVisibility
        {
            get => _systemAdminVisibility;
            set
            {
                if (_systemAdminVisibility != value)
                {
                    _systemAdminVisibility = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Visibility AccountAdminVisibility
        {
            get => _accountAdminVisibility;
            set
            {
                if (_accountAdminVisibility != value)
                {
                    _accountAdminVisibility = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
