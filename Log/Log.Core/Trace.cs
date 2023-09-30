using BrassLoon.CommonCore;
using BrassLoon.Log.Data;
using BrassLoon.Log.Data.Models;
using BrassLoon.Log.Framework;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Log.Core
{
    public class Trace : ITrace
    {
        private readonly TraceData _data;
        private readonly ITraceDataSaver _dataSaver;
        private IEventId _eventId;

        public Trace(TraceData data,
            ITraceDataSaver dataSaver,
            IEventId eventId)
        {
            _data = data;
            _dataSaver = dataSaver;
            _eventId = eventId;
        }

        public Trace(TraceData data,
            ITraceDataSaver dataSaver)
            : this(data, dataSaver, eventId: null)
        {}

        public long TraceId => _data.TraceId;

        public Guid DomainId => _data.DomainId;

        public string EventCode => _data.EventCode;

        public string Message { get => _data.Message; set => _data.Message = value; }
        public dynamic Data 
        {
            get
            {
                if (!string.IsNullOrEmpty(_data.Data))
                    return JsonConvert.DeserializeObject(_data.Data);
                else
                    return null;
            }
            set
            {
                if (value != null)
                    _data.Data = JsonConvert.SerializeObject(value);
                else
                    _data.Data = null;
            }
        }

        public DateTime CreateTimestamp => _data.CreateTimestamp;
        private Guid? EventId { get => _data.EventId; set => _data.EventId = value; }
        public string Category { get => _data.Category; set => _data.Category = value; }
        public string Level { get => _data.Level; set => _data.Level = value; }

        public async Task Create(ITransactionHandler transactionHandler)
        {
            if (_eventId != null)
            {
                await  _eventId.Create(transactionHandler);
                EventId = _eventId.EventId;
            }
            await _dataSaver.Create(transactionHandler, _data);
        }
    }
}
