using BrassLoon.Interface.Log.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Text;

namespace BrassLoon.Client.ViewModel
{
    public class TraceVM : ViewModelBase
    {
        private readonly Trace _trace;

        public TraceVM(Trace trace)
        {
            _trace = trace;
        }

        public long? TraceId => _trace.TraceId;

        public Guid? DomainId => _trace.DomainId;

        public string EventCode => _trace.EventCode;

        public string Message => _trace.Message;

        public dynamic Data => _trace.Data;

        public string DataText
        {
            get
            {
                StringBuilder text = new StringBuilder();
                if (_trace.Data is IEnumerable enumberable)
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

        public DateTime? CreateTimestamp => _trace.CreateTimestamp?.ToLocalTime();

        public string EventName => _trace.EventId?.Name ?? string.Empty;

        public string Category => _trace.Category;

        public string Level => _trace.Level;
    }
}
