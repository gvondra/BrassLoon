using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Account;
using BrassLoon.Interface.Account.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Models = BrassLoon.Interface.Account.Models;

namespace BrassLoon.Client.Behaviors
{
    public class ClientSaver : ICommand
    {
        private ISettingsFactory _settingsFactory;
        private readonly IClientService _clientService;
        private bool _canExecute = true;

        public ClientSaver(ISettingsFactory settingsFactory, IClientService clientService)
        {
            _settingsFactory = settingsFactory;
            _clientService = clientService;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute;

        public void Execute(object parameter)
        {
            try
            {
                if (parameter == null)
                    throw new ArgumentNullException(nameof(parameter));
                if (parameter is ClientVM clientVM && !clientVM.HasErrors)
                {
                    _canExecute = false;
                    CanExecuteChanged.Invoke(this, new EventArgs());
                    Task.Run(() => Save(clientVM.InnerClient, clientVM.Secret))
                        .ContinueWith(SaveCallback, clientVM, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex);
            }
        }

        private Models.Client Save(Models.Client client, string secret)
        {
            Func<ISettings, ClientCredentialRequest, Task<Models.Client>> save = _clientService.Update;
            if (!client.ClientId.HasValue)
                save = _clientService.Create;
            ClientCredentialRequest request = new ClientCredentialRequest
            {
                AccountId = client.AccountId,
                ClientId = client.ClientId,
                IsActive = client.IsActive,
                Name = client.Name,
                Secret = secret
            };
            return save(_settingsFactory.CreateAccountSettings(), request).Result;
        }

        private async Task SaveCallback(Task<Models.Client> save, object state)
        {
            try
            {
                Models.Client client = await save;
                if (state != null && state is ClientVM clientVM)
                {
                    if (!clientVM.ClientId.HasValue)
                        clientVM.ClientId = client.ClientId;
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
