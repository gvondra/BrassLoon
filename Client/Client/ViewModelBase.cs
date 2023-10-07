using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BrassLoon.Client
{
    public abstract class ViewModelBase : INotifyPropertyChanged, IDataErrorInfo
    {
        private readonly ConcurrentDictionary<string, string> _errors = new ConcurrentDictionary<string, string>();
        protected readonly List<object> _behaviors = new List<object>();

        public event PropertyChangedEventHandler PropertyChanged;

        public bool HasErrors => _errors.Any(pair => !string.IsNullOrEmpty(pair.Value));

        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get => _errors.ContainsKey(columnName) ? _errors[columnName] : null;
            set
            {
                _errors[columnName] = value;
                NotifyPropertyChanged(nameof(HasErrors));
            }
        }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool ContainsBehavior<T>()
        {
            List<object> behaviors = _behaviors ?? new List<object>();
            return behaviors.Any(b => b.GetType().Equals(typeof(T)));
        }

        public T GetBehavior<T>()
        {
            List<object> behaviors = _behaviors ?? new List<object>();
            return behaviors.Where(b => b.GetType().Equals(typeof(T))).Select<object, T>(b => (T)b).FirstOrDefault();
        }

        public void AddBehavior(object behavior) => _behaviors.Add(behavior);
    }
}
