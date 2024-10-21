using BrassLoon.Client.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace BrassLoon.Client.NavigationPage
{
    /// <summary>
    /// Interaction logic for ExceptionDetails.xaml
    /// </summary>
    public partial class ExceptionDetails : Page
    {
        public ExceptionDetails()
            : this(null)
        { }
        public ExceptionDetails(ExceptionVM exceptionVM)
        {
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            InitializeComponent();
            DataContext = ToDataContext(exceptionVM)?.ToList();
        }

        private static IEnumerable<object> ToDataContext(ExceptionVM exceptionVM)
        {
            IEnumerable<object> result = null;
            if (exceptionVM != null)
            {
                result = new List<object> { exceptionVM };
                if (exceptionVM.InnerException != null)
                {
                    result = result.Concat(ToDataContext(exceptionVM.InnerException));
                }
            }
            return result;
        }
    }
}
