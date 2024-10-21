using BrassLoon.Client.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace BrassLoon.Client.NavigationPage
{
    /// <summary>
    /// Interaction logic for TaskType.xaml
    /// </summary>
    public partial class TaskType : Page
    {
        public TaskType()
            : this(null)
        { }

        public TaskType(WorkTaskTypeVM taskTypeVM)
        {
            InitializeComponent();
            TaskTypeVM = taskTypeVM;
            DataContext = taskTypeVM;
        }

        public WorkTaskTypeVM TaskTypeVM { get; private set; }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (sender is ListView && TaskTypeVM.SelectedTaskStatus != null)
                {
                    NavigationService navigationService = NavigationService.GetNavigationService(this);
                    TaskStatus taskStatus = new TaskStatus(TaskTypeVM.SelectedTaskStatus);
                    _ = navigationService.Navigate(taskStatus);
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }
    }
}
