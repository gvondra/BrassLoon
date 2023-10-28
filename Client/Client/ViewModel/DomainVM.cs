using BrassLoon.Interface.Account.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BrassLoon.Client.ViewModel
{
    public class DomainVM : ViewModelBase
    {
        private readonly Domain _domain;
        private readonly AppSettings _appSettings;
        private ICommand _save;
        private ICommand _delete;
        private ICommand _exceptionLoad;
        private ICommand _moreExceptionLoad;
        private ICommand _traceLoad;
        private ICommand _moreTraceLoad;
        private ICommand _metricLoad;
        private ICommand _moreMetricLoad;
        private ICommand _roleAdd;
        private ICommand _clientAdd;
        private ICommand _signingKeyAdd;
        private bool _isLoadingExceptions;
        private DateTime _exceptionsMaxTimestamp;
        private bool _isLoadingTraces;
        private DateTime _tracesMaxTimestamp;
        private string _selectedTraceEventCode;
        private bool _isLoadingMetrics;
        private DateTime _metricsMaxTimestamp;
        private string _selectedMetricEventCode;
        private bool _isLoadingRoles;
        private RoleVM _selectedRole;
        private bool _isLoadingClients;
        private DomainClientVM _selectedClient;
        private bool _isLoadingSigningKeys;
        private DomainSigningKeyVM _selectedSigningKey;
        private WorkTaskTypesVM _taskTypes;

        public DomainVM(Domain domain, AppSettings appSettings)
        {
            _domain = domain;
            _exceptionsMaxTimestamp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            _exceptionsMaxTimestamp = _exceptionsMaxTimestamp.AddMinutes(1);
            _tracesMaxTimestamp = _exceptionsMaxTimestamp;
            _metricsMaxTimestamp = _exceptionsMaxTimestamp;
            _appSettings = appSettings;
        }

        internal Domain InnerDomain => _domain;

        public ObservableCollection<ExceptionVM> Exceptions { get; } = new ObservableCollection<ExceptionVM>();
        public ObservableCollection<TraceVM> Traces { get; } = new ObservableCollection<TraceVM>();
        public ObservableCollection<string> TraceEventCodes { get; } = new ObservableCollection<string>();
        public ObservableCollection<MetricVM> Metrics { get; } = new ObservableCollection<MetricVM>();
        public ObservableCollection<string> MetricEventCodes { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> ItemCodes { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> LookupCodes { get; } = new ObservableCollection<string>();
        public ObservableCollection<RoleVM> Roles { get; } = new ObservableCollection<RoleVM>();
        public ObservableCollection<DomainClientVM> Clients { get; } = new ObservableCollection<DomainClientVM>();
        public ObservableCollection<DomainSigningKeyVM> SigningKeys { get; } = new ObservableCollection<DomainSigningKeyVM>();

        public Guid DomainId => _domain.DomainId.Value;

        public string JWKS => string.Format("{0}/{1:D}", _appSettings.JwksBaseAddress.TrimEnd('/'), DomainId);

        public WorkTaskTypesVM TaskTypes
        {
            get => _taskTypes;
            set
            {
                if (_taskTypes != value)
                {
                    _taskTypes = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsLoadingSigningKeys
        {
            get => _isLoadingSigningKeys;
            set
            {
                if (_isLoadingSigningKeys != value)
                {
                    _isLoadingSigningKeys = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DomainSigningKeyVM SelectedSigningKey
        {
            get => _selectedSigningKey;
            set
            {
                if (_selectedSigningKey != value)
                {
                    _selectedSigningKey = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DomainClientVM SelectedClient
        {
            get => _selectedClient;
            set
            {
                if (_selectedClient != value)
                {
                    _selectedClient = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsLoadingClients
        {
            get => _isLoadingClients;
            set
            {
                if (_isLoadingClients != value)
                {
                    _isLoadingClients = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public RoleVM SelectedRole
        {
            get => _selectedRole;
            set
            {
                if (_selectedRole != value)
                {
                    _selectedRole = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsLoadingRoles
        {
            get => _isLoadingRoles;
            set
            {
                if (_isLoadingRoles != value)
                {
                    _isLoadingRoles = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string SelectedTraceEventCode
        {
            get => _selectedTraceEventCode;
            set
            {
                if (_selectedTraceEventCode != value)
                {
                    _selectedTraceEventCode = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DateTime ExceptionsMaxTimestamp
        {
            get => _exceptionsMaxTimestamp;
            set
            {
                if (_exceptionsMaxTimestamp != value)
                {
                    _exceptionsMaxTimestamp = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsLoadingExceptions
        {
            get => _isLoadingExceptions;
            set
            {
                if (_isLoadingExceptions != value)
                {
                    _isLoadingExceptions = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string SelectedMetricEventCode
        {
            get => _selectedMetricEventCode;
            set
            {
                if (_selectedMetricEventCode != value)
                {
                    _selectedMetricEventCode = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DateTime MetricsMaxTimestamp
        {
            get => _metricsMaxTimestamp;
            set
            {
                if (_metricsMaxTimestamp != value)
                {
                    _metricsMaxTimestamp = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsLoadingMetrics
        {
            get => _isLoadingMetrics;
            set
            {
                if (_isLoadingMetrics != value)
                {
                    _isLoadingMetrics = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DateTime TracesMaxTimestamp
        {
            get => _tracesMaxTimestamp;
            set
            {
                if (_tracesMaxTimestamp != value)
                {
                    _tracesMaxTimestamp = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsLoadingTraces
        {
            get => _isLoadingTraces;
            set
            {
                if (_isLoadingTraces != value)
                {
                    _isLoadingTraces = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Name
        {
            get => _domain.Name;
            set
            {
                if (_domain.Name != value)
                {
                    _domain.Name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand Save
        {
            get => _save;
            set
            {
                if (_save != value)
                {
                    _save = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand Delete
        {
            get => _delete;
            set
            {
                if (_delete != value)
                {
                    _delete = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand ExceptionLoad
        {
            get => _exceptionLoad;
            set
            {
                if (_exceptionLoad != value)
                {
                    _exceptionLoad = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand MoreExceptionLoad
        {
            get => _moreExceptionLoad;
            set
            {
                if (_moreExceptionLoad != value)
                {
                    _moreExceptionLoad = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand TraceLoad
        {
            get => _traceLoad;
            set
            {
                if (_traceLoad != value)
                {
                    _traceLoad = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand MoreTraceLoad
        {
            get => _moreTraceLoad;
            set
            {
                if (_moreTraceLoad != value)
                {
                    _moreTraceLoad = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand MetricLoad
        {
            get => _metricLoad;
            set
            {
                if (_metricLoad != value)
                {
                    _metricLoad = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand MoreMetricLoad
        {
            get => _moreMetricLoad;
            set
            {
                if (_moreMetricLoad != value)
                {
                    _moreMetricLoad = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand RoleAdd
        {
            get => _roleAdd;
            set
            {
                if (_roleAdd != value)
                {
                    _roleAdd = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand ClientAdd
        {
            get => _clientAdd;
            set
            {
                if (_clientAdd != value)
                {
                    _clientAdd = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand SigningKeyAdd
        {
            get => _signingKeyAdd;
            set
            {
                if (_signingKeyAdd != value)
                {
                    _signingKeyAdd = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
