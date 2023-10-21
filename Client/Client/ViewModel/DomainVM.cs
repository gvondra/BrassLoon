using BrassLoon.Interface.Account.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BrassLoon.Client.ViewModel
{
    public class DomainVM : ViewModelBase
    {
        private readonly Domain _domain;
        private ICommand _save;
        private ICommand _delete;
        private ICommand _exceptionLoad;
        private ICommand _moreExceptionLoad;
        private bool _isLoadingExceptions;
        private DateTime _exceptionsMaxTimestamp;

        public DomainVM(Domain domain)
        {
            _domain = domain;
            _exceptionsMaxTimestamp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            _exceptionsMaxTimestamp = _exceptionsMaxTimestamp.AddMinutes(1);
        }

        internal Domain InnerDomain => _domain;

        public ObservableCollection<ExceptionVM> Exceptions { get; } = new ObservableCollection<ExceptionVM>();

        public Guid DomainId => _domain.DomainId.Value;

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
    }
}
