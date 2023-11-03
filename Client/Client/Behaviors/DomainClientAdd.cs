using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Models = BrassLoon.Interface.Authorization.Models;

namespace BrassLoon.Client.Behaviors
{
    public class DomainClientAdd : ICommand
    {
        private readonly ISettingsFactory _settingsFactory;
        private readonly IRoleService _roleService;
        private readonly IClientService _clientService;
        private bool _canExecute = true;

        public DomainClientAdd(ISettingsFactory settingsFactory, IRoleService roleService, IClientService clientService)
        {
            _settingsFactory = settingsFactory;
            _roleService = roleService;
            _clientService = clientService;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute;

        public void Execute(object parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));
            if (parameter is DomainVM domainVM)
            {
                _canExecute = false;
                CanExecuteChanged.Invoke(this, new EventArgs());
                Task.Run(() => _roleService.GetByDomainId(_settingsFactory.CreateAuthorizationSettings(), domainVM.DomainId).Result)
                    .ContinueWith(GetRolesCallback, domainVM, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private async Task GetRolesCallback(Task<List<Models.Role>> getRoles, object state)
        {
            try
            {
                List<Models.Role> roles = await getRoles;
                if (state is DomainVM domainVM)
                {
                    DomainClientVM clientVM = new DomainClientVM(new Models.Client { DomainId = domainVM.DomainId }, domainVM);
                    clientVM.IsActive = true;
                    clientVM.Name = "New Client";
                    foreach (Models.Role role in roles)
                    {
                        clientVM.AppliedRoles.Add(new AppliedRoleVM(new Models.AppliedRole { Name = role.Name, PolicyName = role.PolicyName }));
                    }
                    clientVM.GenerateSecret = new DomainClientSecretGenerator(_settingsFactory, _clientService);
                    domainVM.Clients.Add(clientVM);
                    domainVM.SelectedClient = clientVM;
                    clientVM.GenerateSecret.Execute(clientVM);
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex);
            }
            finally
            {
                _canExecute = true;
                CanExecuteChanged.Invoke(this, new EventArgs());
            }
        }
    }
}
