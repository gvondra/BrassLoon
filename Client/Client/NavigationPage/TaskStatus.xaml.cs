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
