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
