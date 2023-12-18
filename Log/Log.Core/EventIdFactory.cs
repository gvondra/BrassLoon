using BrassLoon.CommonCore;
using BrassLoon.Log.Data;
using BrassLoon.Log.Data.Models;
using BrassLoon.Log.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Log.Core
{
    public class EventIdFactory : IEventIdFactory
    {
        private readonly IEventIdDataFactory _dataFactory;
        private readonly IEventIdDataSaver _dataSaver;

        public EventIdFactory(IEventIdDataFactory dataFactory, IEventIdDataSaver dataSaver)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
        }

        private EventId Create(EventIdData data) => new EventId(data, _dataSaver);

        public IEventId Create(Guid domainId, int id, string name)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            return Create(new EventIdData { DomainId = domainId, Id = id, Name = name });
        }

        public async Task<IEventId> Get(ISettings settings, Guid id)
        {
            EventId result = null;
            EventIdData data = await _dataFactory.Get(new DataSettings(settings), id);
            if (data != null)
                result = Create(data);
            return result;
        }

        public async Task<IEnumerable<IEventId>> GetByDomainId(ISettings settings, Guid domainId)
        {
            return (await _dataFactory.GetByDomainId(new DataSettings(settings), domainId))
                .Select<EventIdData, IEventId>(Create);
        }
    }
}
