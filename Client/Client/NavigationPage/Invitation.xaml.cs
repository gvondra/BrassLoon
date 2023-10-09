using Autofac;
using BrassLoon.Client.Behaviors;
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
    /// Interaction logic for Invitation.xaml
    /// </summary>
    public partial class Invitation : Page
    {
        public Invitation()
            : this(null)
        { }

        internal Invitation(UserInvitationVM invitationVM)
        {
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            InitializeComponent();
            UserInvitationVM = invitationVM;
            DataContext = UserInvitationVM;
            this.Loaded += Invitation_Loaded;
        }

        internal UserInvitationVM UserInvitationVM { get; private set; }

        private void Invitation_Loaded(object sender, RoutedEventArgs e)
        {
            ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifetimeScope();
            if (UserInvitationVM.Cancel == null)
                UserInvitationVM.Cancel = scope.Resolve<InvitationCancel>();
        }

    }
}
