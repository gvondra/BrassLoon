using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Authorization;
using BrassLoon.Interface.Authorization.Models;
using BrassLoon.Interface.Config;
using BrassLoon.Interface.Log;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using AuthModels = BrassLoon.Interface.Authorization.Models;
using LogModels = BrassLoon.Interface.Log.Models;

namespace BrassLoon.Client.Behaviors
{
    public class DomainLoader
    {
        private readonly DomainVM _domainVM;
        private readonly IExceptionService _exceptionService;
        private readonly ITraceService _traceService;
        private readonly IMetricService _metricService;
        private readonly IItemService _itemService;
        private readonly ILookupService _lookupService;
        private readonly IRoleService _roleService;
        private readonly IClientService _clientService;
        private readonly ISigningKeyService _signingKeyService;
        private readonly ISettingsFactory _settingsFactory;

        public DomainLoader(
            DomainVM domainVM,
            IExceptionService exceptionService,
            ITraceService traceService,
            IMetricService metricService,
            IItemService itemService,
            ILookupService lookupService,
            IRoleService roleService,
            IClientService clientService,
            ISigningKeyService signingKeyService,
            ISettingsFactory settingsFactory)
        {
            _domainVM = domainVM;
            _exceptionService = exceptionService;
            _traceService = traceService;
            _metricService = metricService;
            _itemService = itemService;
            _lookupService = lookupService;
            _roleService = roleService;
            _clientService = clientService;
            _signingKeyService = signingKeyService;
            _domainVM.SigningKeys.Clear();
            _settingsFactory = settingsFactory;
            domainVM.Roles.CollectionChanged += Roles_CollectionChanged;
            domainVM.Clients.CollectionChanged += Clients_CollectionChanged;
        }

        public void LoadExceptions()
        {
            _domainVM.IsLoadingExceptions = true;
            _domainVM.Exceptions.Clear();
            Task.Run(() => _exceptionService.Search(_settingsFactory.CreateLogSettings(), _domainVM.DomainId, _domainVM.ExceptionsMaxTimestamp.ToUniversalTime()))
                .ContinueWith(LoadExceptionsCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task LoadExceptionsCallback(Task<List<LogModels.Exception>> getExceptions, object state)
        {
            try
            {
                List<LogModels.Exception> exceptions = await getExceptions;
                _domainVM.Exceptions.Clear();
                foreach (LogModels.Exception exception in exceptions)
                {
                    _domainVM.Exceptions.Add(new ExceptionVM(exception));
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
            }
            finally
            {
                _domainVM.IsLoadingExceptions = false;
            }
        }

        public void LoadTraceEventCodes()
        {
            _domainVM.TraceEventCodes.Clear();
            Task.Run(() => _traceService.GetEventCodes(_settingsFactory.CreateLogSettings(), _domainVM.DomainId).Result)
                .ContinueWith(LoadTraceEventCodesCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task LoadTraceEventCodesCallback(Task<List<string>> loadTraceEventCodes, object state)
        {
            try
            {
                _domainVM.TraceEventCodes.Clear();
                foreach (string code in await loadTraceEventCodes)
                {
                    _domainVM.TraceEventCodes.Add(code);
                }
                if (_domainVM.TraceEventCodes.Count > 0)
                {
                    _domainVM.SelectedTraceEventCode = _domainVM.TraceEventCodes[0];
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
            }
        }

        public void LoadMetricEventCodes()
        {
            _domainVM.MetricEventCodes.Clear();
            Task.Run(() => _metricService.GetEventCodes(_settingsFactory.CreateLogSettings(), _domainVM.DomainId).Result)
                .ContinueWith(LoadMetricEventCodesCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task LoadMetricEventCodesCallback(Task<List<string>> loadMetricEventCodes, object state)
        {
            try
            {
                _domainVM.MetricEventCodes.Clear();
                foreach (string code in await loadMetricEventCodes)
                {
                    _domainVM.MetricEventCodes.Add(code);
                }
                if (_domainVM.MetricEventCodes.Count > 0)
                    _domainVM.SelectedMetricEventCode = _domainVM.MetricEventCodes[0];
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
            }
        }

        public void LoadItemCodes()
        {
            _domainVM.ItemCodes.Clear();
            Task.Run(() => _itemService.GetCodes(_settingsFactory.CreateConfigSettings(), _domainVM.DomainId).Result)
                .ContinueWith(LoadItemCodesCodesCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task LoadItemCodesCodesCallback(Task<List<string>> loadMetricEventCodes, object state)
        {
            try
            {
                _domainVM.ItemCodes.Clear();
                foreach (string code in await loadMetricEventCodes)
                {
                    _domainVM.ItemCodes.Add(code);
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
            }
        }

        public void LoadLookupCodes()
        {
            _domainVM.LookupCodes.Clear();
            Task.Run(() => _lookupService.GetCodes(_settingsFactory.CreateConfigSettings(), _domainVM.DomainId).Result)
                .ContinueWith(LoadLookupCodesCodesCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task LoadLookupCodesCodesCallback(Task<List<string>> loadMetricEventCodes, object state)
        {
            try
            {
                _domainVM.LookupCodes.Clear();
                foreach (string code in await loadMetricEventCodes)
                {
                    _domainVM.LookupCodes.Add(code);
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
            }
        }

        public void LoadRoles()
        {
            _domainVM.IsLoadingRoles = true;
            _domainVM.Roles.Clear();
            Task.Run(() => _roleService.GetByDomainId(_settingsFactory.CreateAuthorizationSettings(), _domainVM.DomainId).Result)
                .ContinueWith(LoadRolesCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task LoadRolesCallback(Task<List<Role>> loadRoles, object state)
        {
            try
            {
                List<Role> roles = await loadRoles;
                _domainVM.Roles.Clear();
                foreach (Role role in roles)
                {
                    _domainVM.Roles.Add(new RoleVM(role, _domainVM));
                }
                if (_domainVM.Roles.Count > 0)
                    _domainVM.SelectedRole = _domainVM.Roles[0];
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
            }
            finally
            {
                _domainVM.IsLoadingRoles = false;
            }
        }

        private void Roles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add
                || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (RoleVM roleVM in e.NewItems)
                {
                    if (roleVM.GetBehavior<RoleValidator>() == null)
                    {
                        roleVM.AddBehavior(new RoleValidator(roleVM));
                    }
                    if (roleVM.Save == null)
                        roleVM.Save = new DomainRoleSaver(_settingsFactory, _roleService);
                }
            }
        }

        private void Clients_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add
                || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (DomainClientVM clientVM in e.NewItems)
                {
                    if (clientVM.GetBehavior<DomainClientValidator>() == null)
                        clientVM.AddBehavior(new DomainClientValidator(clientVM));
                    if (clientVM.GenerateSecret == null)
                        clientVM.GenerateSecret = new DomainClientSecretGenerator(_settingsFactory, _clientService);
                    if (clientVM.Save == null)
                        clientVM.Save = new DomainClientSaver(_settingsFactory, _clientService);
                }
            }
        }

        public void LoadClients()
        {
            _domainVM.IsLoadingClients = true;
            _domainVM.Clients.Clear();
            Task.Run(() => _clientService.GetByDomain(_settingsFactory.CreateAuthorizationSettings(), _domainVM.DomainId).Result)
                .ContinueWith(LoadClientsCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task LoadClientsCallback(Task<List<AuthModels.Client>> loadClients, object state)
        {
            try
            {
                List<AuthModels.Client> clients = await loadClients;
                _ = Task.Run(() => _roleService.GetByDomainId(_settingsFactory.CreateAuthorizationSettings(), _domainVM.DomainId))
                    .ContinueWith(LoadClientRollsCallback, clients, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
                _domainVM.IsLoadingClients = false;
            }
        }

        private async Task LoadClientRollsCallback(Task<List<Role>> loadRoles, object state)
        {
            try
            {
                List<Role> roles = await loadRoles;
                List<AuthModels.Client> clients = (List<AuthModels.Client>)state;
                _domainVM.Clients.Clear();
                foreach (AuthModels.Client client in clients)
                {
                    DomainClientVM vm = new DomainClientVM(client, _domainVM);
                    foreach (Role role in roles)
                    {
                        AppliedRoleVM appliedRoleVM = new AppliedRoleVM(
                            new AppliedRole { Name = role.Name, PolicyName = role.PolicyName });
                        appliedRoleVM.IsApplied = client.Roles.Exists(r => string.Equals(r.PolicyName, role.PolicyName, System.StringComparison.OrdinalIgnoreCase));
                        vm.AppliedRoles.Add(appliedRoleVM);
                    }
                    _domainVM.Clients.Add(vm);
                }
                if (_domainVM.Clients.Count > 0)
                {
                    _domainVM.SelectedClient = _domainVM.Clients[0];
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
            }
            finally
            {
                _domainVM.IsLoadingClients = false;
            }
        }

        public void LoadSigningKeys()
        {
            _domainVM.IsLoadingSigningKeys = true;
            _domainVM.SigningKeys.Clear();
            Task.Run(() => _signingKeyService.GetByDomain(_settingsFactory.CreateAuthorizationSettings(), _domainVM.DomainId).Result)
                .ContinueWith(LoadSigningKeysCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task LoadSigningKeysCallback(Task<List<SigningKey>> loadSigningKeys, object state)
        {
            try
            {
                List<SigningKey> signingKeys = await loadSigningKeys;
                _domainVM.SigningKeys.Clear();
                foreach (SigningKey signingKey in signingKeys)
                {
                    _domainVM.SigningKeys.Add(new DomainSigningKeyVM(signingKey, _domainVM));
                }
                if (_domainVM.SigningKeys.Count > 0)
                    _domainVM.SelectedSigningKey = _domainVM.SigningKeys[0];
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
            }
            finally
            {
                _domainVM.IsLoadingSigningKeys = false;
            }
        }
    }
}
