using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Account;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BrassLoon.Client.Behaviors
{
    public class ClientSecretGenreator : ICommand
    {
        private readonly ISettingsFactory _settingsFactory;
        private readonly IClientService _clientService;
        private bool _canExecute = true;

        public ClientSecretGenreator(ISettingsFactory settingsFactory, IClientService clientService)
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
            if (parameter is ClientVM clientVM)
            {
                _canExecute = false;
                CanExecuteChanged.Invoke(this, new EventArgs());
                _ = Task.Run(GenerateSecret)
                    .ContinueWith(GenerateSecretCallback, clientVM, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private string GenerateSecret()
        => _clientService.CreateSecret(_settingsFactory.CreateAccountSettings()).Result;

        private async Task GenerateSecretCallback(Task<string> generateSecret, object state)
        {
            try
            {
                if (state is ClientVM clientVM)
                {
                    clientVM.Secret = await generateSecret;
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
