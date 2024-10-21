using BrassLoon.Client.ViewModel;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace BrassLoon.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private MainWindowVM MainWindowVM { get; set; }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindowVM = new MainWindowVM();
            DataContext = MainWindowVM;
        }

        private void CloseCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e) => Close();

        private void GoToPageCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NavigationService navigationService = navigationFrame.NavigationService;
            JournalEntry journalEntry = navigationService.RemoveBackEntry();
            while (journalEntry != null)
                journalEntry = navigationService.RemoveBackEntry();
            //NavigationService navigationService = NavigationService.GetNavigationService(navigationFrame);
            _ = navigationService.Navigate(new Uri((string)e.Parameter, UriKind.Relative));
        }

        private void GoogleLoginMenuItem_Click(object sender, RoutedEventArgs e)
        => GoogleLogin.ShowLoginDialog(checkAccessToken: false, owner: this);
    }
}
