using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Models = BrassLoon.Interface.Authorization.Models;

namespace BrassLoon.Client.Behaviors
{
    public class DomainClientSaver : ICommand
    {
        private readonly ISettingsFactory _settingsFactory;
        private readonly IClientService _clientService;
        private bool _canExecute = true;

        public DomainClientSaver(ISettingsFactory settingsFactory, IClientService clientService)
        {
            _settingsFactory = settingsFactory;
            _clientService = clientService;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute;

        public void Execute(object parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));
            if (parameter is DomainClientVM clientVM && !clientVM.HasErrors)
            {
                _canExecute = false;
                CanExecuteChanged.Invoke(this, new EventArgs());
                Task.Run(() =>
                {
                    Func<ISettings, Models.Client, Task<Models.Client>> save = _clientService.Update;
                    if (clientVM.IsNew)
                        save = _clientService.Create;
                    Models.Client client = clientVM.InnerClient;
                    client.Roles = clientVM.AppliedRoles
                        .Where(r => r.IsApplied)
                        .Select<AppliedRoleVM, Models.AppliedRole>(r => new Models.AppliedRole { Name = r.Name, PolicyName = r.PolicyName})
                        .ToList();
                    return save(_settingsFactory.CreateAuthorizationSettings(), client).Result;
                })
                    .ContinueWith(SaveCallback, clientVM, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private async Task SaveCallback(Task<Models.Client> save, object state)
        {
            try
            {
                Models.Client client = await save;
                if (state is DomainClientVM clientVM && clientVM.IsNew)
                {
                    int index = clientVM.DomainVM.Clients.IndexOf(clientVM);
                    if (index >= 0)
                    {
                        DomainClientVM newClientVM = new DomainClientVM(client, clientVM.DomainVM);
                        foreach (AppliedRoleVM appliedRoleVM in clientVM.AppliedRoles)
                        {
                            newClientVM.AppliedRoles.Add(appliedRoleVM);
                        }
                        clientVM.DomainVM.Clients[index] = newClientVM;
                        clientVM.DomainVM.SelectedClient = newClientVM;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex);
            }
            finally
            {
                if (CanExecuteChanged != null)
                {
                    _canExecute = true;
                    CanExecuteChanged.Invoke(this, new EventArgs());
                }
            }
        }
    }
}
