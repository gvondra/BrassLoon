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
    /// Interaction logic for CreateInvitation.xaml
    /// </summary>
    public partial class CreateInvitation : Page
    {
        public CreateInvitation()
            : this(null)
        { }

        internal CreateInvitation(AccountVM accountVM)
        {
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            InitializeComponent();
            CreateInvitationVM = new CreateInvitationVM(accountVM);
            DataContext = CreateInvitationVM;
            this.Loaded += CreateInvitation_Loaded;
        }

        internal CreateInvitationVM CreateInvitationVM { get; private set; }

        private void CreateInvitation_Loaded(object sender, RoutedEventArgs e)
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifetimeScope();
            if (CreateInvitationVM.GetBehavior<CreateInvitationValidator>() == null)
                CreateInvitationVM.AddBehavior(scope.Resolve<Func<CreateInvitationVM, CreateInvitationValidator>>()(CreateInvitationVM));
            if (CreateInvitationVM.Create == null)
                CreateInvitationVM.Create = scope.Resolve<InvitationCreator>();
            EmailAddress.Focus();
        }
    }
}
