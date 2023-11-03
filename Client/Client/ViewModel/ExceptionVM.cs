using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Text;
using System.Windows.Documents;
using Models = BrassLoon.Interface.Log.Models;

namespace BrassLoon.Client.ViewModel
{
    public class ExceptionVM : ViewModelBase
    {
        private readonly Models.Exception _exception;
        private readonly ExceptionVM _innerException;

        public ExceptionVM(Models.Exception exception)
        {
            _exception = exception;
            if (exception.InnerException != null)
                _innerException = new ExceptionVM(exception.InnerException);
        }

        public long? ExceptionId => _exception.ExceptionId;

        public Guid? DomainId => _exception.DomainId;

        public string Message => _exception.Message;

        public string TypeName => _exception.TypeName;

        public string Source => _exception.Source;

        public string AppDomain => _exception.AppDomain;

        public string TargetSite => _exception.TargetSite;

        public string StackTrace => _exception.StackTrace;

        public dynamic Data => _exception.Data;

        public string DataText
        {
            get
            {
                StringBuilder text = new StringBuilder();
                if (_exception.Data is IEnumerable enumberable)
                {
                    IEnumerator enumerator = enumberable.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current is JProperty jProperty)
                        {
                            text.AppendLine($"{jProperty.Name} : \t{jProperty.Value}");
                        }
                    }
                }
                return text.ToString().Trim();
            }
        }

        public DateTime? CreateTimestamp => _exception.CreateTimestamp?.ToLocalTime();

        public ExceptionVM InnerException => _innerException;

        public string EventName => _exception.EventId?.Name ?? string.Empty;

        public string Category => _exception.Category;

        public string Level => _exception.Level;
    }
}
