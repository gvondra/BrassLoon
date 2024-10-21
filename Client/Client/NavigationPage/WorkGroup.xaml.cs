using BrassLoon.Client.ViewModel;
using System.Windows.Controls;

namespace BrassLoon.Client.NavigationPage
{
    /// <summary>
    /// Interaction logic for WorkGroup.xaml
    /// </summary>
    public partial class WorkGroup : Page
    {
        public WorkGroup()
            : this(null)
        { }

        public WorkGroup(WorkGroupVM workGroupVM)
        {
            InitializeComponent();
            WorkGroupVM = workGroupVM;
            DataContext = workGroupVM;
        }

        public WorkGroupVM WorkGroupVM { get; private set; }
    }
}
