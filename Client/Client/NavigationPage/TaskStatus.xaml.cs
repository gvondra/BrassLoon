using BrassLoon.Client.ViewModel;
using System.Windows.Controls;

namespace BrassLoon.Client.NavigationPage
{
    /// <summary>
    /// Interaction logic for TaskStatus.xaml
    /// </summary>
    public partial class TaskStatus : Page
    {
        public TaskStatus()
            : this(null)
        { }

        public TaskStatus(WorkTaskStatusVM taskStatusVM)
        {
            InitializeComponent();
            TaskStatusVM = taskStatusVM;
            DataContext = TaskStatusVM;
        }

        public WorkTaskStatusVM TaskStatusVM { get; private set; }
    }
}
