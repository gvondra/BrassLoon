using BrassLoon.CommonCore;
using BrassLoon.Log.Data;
using BrassLoon.Log.Data.Models;
using BrassLoon.Log.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Core
{
    public class Metric : IMetric
    {
        private readonly MetricData _data;
        private readonly IMetricDataSaver _dataSaver;

        public Metric(MetricData data,
            IMetricDataSaver dataSaver)
        {
            _data = data;
            _dataSaver = dataSaver;
        }

        public long MetricId => _data.MetricId;

        public Guid DomainId => _data.DomainId;

        public string EventCode => _data.EventCode;

        public double? Magnitude { get => _data.Magnitude; set => _data.Magnitude = value; }
        public dynamic Data 
        { 
            get => JsonConvert.DeserializeObject(_data.Data);
            set
            {
                if (value != null)
                    _data.Data = JsonConvert.SerializeObject(value);
                else
                    _data.Data = null;
            }
        }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public async Task Create(ITransactionHandler transactionHandler)
        {
            await _dataSaver.Create(transactionHandler, _data);
        }
    }
}
