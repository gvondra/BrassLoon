using BrassLoon.Interface.Log.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Text;

namespace BrassLoon.Client.ViewModel
{
    public class MetricVM : ViewModelBase
    {
        private readonly Metric _metric;

        public MetricVM(Metric metric)
        {
            _metric = metric;
        }

        public long? MetricId => _metric.MetricId;

        public Guid? DomainId => _metric.DomainId;

        public string EventCode => _metric.EventCode;

        public double? Magnitude => _metric.Magnitude;

        public dynamic Data => _metric.Data;

        public string DataText
        {
            get
            {
                StringBuilder text = new StringBuilder();
                if (_metric.Data is IEnumerable enumberable)
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

        public DateTime? CreateTimestamp => _metric.CreateTimestamp?.ToLocalTime();

        public string Status => _metric.Status;

        public string Requestor => _metric.Requestor;

        public string EventName => _metric.EventId?.Name ?? string.Empty;

        public string Category => _metric.Category;

        public string Level => _metric.Level;
    }
}
