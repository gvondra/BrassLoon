﻿using Autofac;
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
    /// Interaction logic for Account.xaml
    /// </summary>
    public partial class Account : Page
    {
        public Account()
        {
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            InitializeComponent();
            DataContext = null;
            AccountVM = null;
            this.Loaded += Account_Loaded;
        }

        internal Account(AccountVM accountVM)
            : this()
        {
            AccountVM = accountVM;
            DataContext = accountVM;
            this.Loaded += Account_Loaded;
        }

        internal AccountVM AccountVM { get; private set; }

        private void Account_Loaded(object sender, RoutedEventArgs e)
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifetimeScope();
            if (AccountVM.GetBehavior<AccountValidator>() == null)
                AccountVM.AddBehavior(new AccountValidator(AccountVM));
            if (AccountVM.SaveCommand == null)
                AccountVM.SaveCommand = scope.Resolve<AccountSaver>();
        }
    }
}
