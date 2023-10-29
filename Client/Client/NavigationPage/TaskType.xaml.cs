using BrassLoon.Client.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
                if (sender is ListView listView && TaskTypeVM.SelectedTaskStatus != null)
                {
                    NavigationService navigationService = NavigationService.GetNavigationService(this);
                    NavigationPage.TaskStatus taskStatus = new NavigationPage.TaskStatus(TaskTypeVM.SelectedTaskStatus);
                    navigationService.Navigate(taskStatus);
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }
    }
}
