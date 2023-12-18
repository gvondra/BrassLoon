using BrassLoon.CommonCore;
using BrassLoon.Log.Data;
using BrassLoon.Log.Data.Models;
using BrassLoon.Log.Framework;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Log.Core
{
    public class EventId : IEventId
    {
        private readonly EventIdData _data;
        private readonly IEventIdDataSaver _dataSaver;

        public EventId(EventIdData data, IEventIdDataSaver dataSaver)
        {
            _data = data;
            _dataSaver = dataSaver;
        }

        public Guid DomainId => _data.DomainId;

        public int Id => _data.Id;

        public string Name => _data.Name;

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        Guid IEventId.EventId => _data.EventId;

        public Task Create(ITransactionHandler transactionHandler) => _dataSaver.Create(transactionHandler, _data);
    }
}
