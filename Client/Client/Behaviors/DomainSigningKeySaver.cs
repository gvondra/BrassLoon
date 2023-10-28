using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Authorization;
using BrassLoon.Interface.Authorization.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BrassLoon.Client.Behaviors
{
    public class DomainSigningKeySaver : ICommand
    {
        private readonly ISettingsFactory _settingsFactory;
        private readonly ISigningKeyService _signingKeyService;
        private bool _canExecute = true;

        public DomainSigningKeySaver(ISettingsFactory settingsFactory, ISigningKeyService signingKeyService)
        {
            _settingsFactory = settingsFactory;
            _signingKeyService = signingKeyService;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute;

        public void Execute(object parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));
            if (parameter is DomainSigningKeyVM signingKeyVM && !signingKeyVM.HasErrors)
            {
                _canExecute = false;
                CanExecuteChanged.Invoke(this, new EventArgs());
                Task.Run(() =>
                {
                    Func<ISettings, SigningKey, Task<SigningKey>> save = _signingKeyService.Update;
                    if (!signingKeyVM.SigningKeyId.HasValue)
                        save = _signingKeyService.Create;
                    return save(_settingsFactory.CreateAuthorizationSettings(), signingKeyVM.InnerSigningKey).Result;
                })
                    .ContinueWith(SaveCallback, signingKeyVM, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private async Task SaveCallback(Task<SigningKey> save, object state)
        {
            try
            {
                SigningKey signingKey = await save;
                if (state is DomainSigningKeyVM signingKeyVM && !signingKeyVM.SigningKeyId.HasValue)
                {
                    int index = signingKeyVM.DomainVM.SigningKeys.IndexOf(signingKeyVM);
                    if (index >= 0)
                    {
                        DomainSigningKeyVM newSigningKeyVM = new DomainSigningKeyVM(signingKey, signingKeyVM.DomainVM);
                        signingKeyVM.DomainVM.SigningKeys[index] = newSigningKeyVM;
                        signingKeyVM.DomainVM.SelectedSigningKey = newSigningKeyVM;
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
