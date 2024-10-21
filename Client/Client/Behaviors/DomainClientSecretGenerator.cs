using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Authorization;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BrassLoon.Client.Behaviors
{
    public class DomainClientSecretGenerator : ICommand
    {
        private readonly ISettingsFactory _settingsFactory;
        private readonly IClientService _clientService;
        private bool _canExecute = true;

        public DomainClientSecretGenerator(ISettingsFactory settingsFactory, IClientService clientService)
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
            if (parameter is DomainClientVM domainClientVM)
            {
                _canExecute = false;
                CanExecuteChanged.Invoke(this, new EventArgs());
                _ = Task.Run(() => _clientService.GetClientCredentialSecret(_settingsFactory.CreateAuthorizationSettings()).Result)
                    .ContinueWith(GenerateSecretCallback, domainClientVM, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private async Task GenerateSecretCallback(Task<string> generateSecret, object state)
        {
            try
            {
                string secret = await generateSecret;
                if (state is DomainClientVM clientVM)
                {
                    clientVM.Secret = secret;
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
