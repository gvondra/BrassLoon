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
    public class Trace : ITrace
    {
        private readonly TraceData _data;
        private readonly ITraceDataSaver _dataSaver;

        public Trace(TraceData data,
            ITraceDataSaver dataSaver)
        {
            _data = data;
            _dataSaver = dataSaver;
        }

        public long TraceId => _data.TraceId;

        public Guid DomainId => _data.DomainId;

        public string EventCode => _data.EventCode;

        public string Message { get => _data.Message; set => _data.Message = value; }
        public dynamic Data 
        { 
            get => JsonConvert.DeserializeObject(_data.Data); 
            set => _data.Data = JsonConvert.SerializeObject(value); 
        }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public async Task Create(ITransactionHandler transactionHandler)
        {
            await _dataSaver.Create(transactionHandler, _data);
        }
    }
}
