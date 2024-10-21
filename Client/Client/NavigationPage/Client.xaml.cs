using Autofac;
using BrassLoon.Client.Behaviors;
using BrassLoon.Client.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BrassLoon.Client.NavigationPage
{
    /// <summary>
    /// Interaction logic for Client.xaml
    /// </summary>
    public partial class Client : Page
    {
        public Client()
            : this(null)
        { }

        public Client(ClientVM clientVM)
        {
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            InitializeComponent();
            ClientVM = clientVM;
            DataContext = clientVM;
            Loaded += Client_Loaded;
        }

        internal ClientVM ClientVM { get; private set; }

        private void Client_Loaded(object sender, RoutedEventArgs e)
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifetimeScope();
            if (ClientVM.GenerateSecret == null)
                ClientVM.GenerateSecret = scope.Resolve<ClientSecretGenreator>();
            if (ClientVM.Save == null)
                ClientVM.Save = scope.Resolve<ClientSaver>();
            if (!ClientVM.ClientId.HasValue)
                ClientVM.GenerateSecret.Execute(ClientVM);
            if (ClientVM.GetBehavior<ClientValidator>() == null)
            {
                ClientVM.AddBehavior(
                    scope.Resolve<Func<ClientVM, ClientValidator>>()(ClientVM));
            }
        }
    }
}
