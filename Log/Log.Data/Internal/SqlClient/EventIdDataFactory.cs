using BrassLoon.DataClient;
using BrassLoon.Log.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data.Internal.SqlClient
{
    public class EventIdDataFactory : IEventIdDataFactory
    {
        private readonly ISqlDbProviderFactory _providerFactory;
        private readonly GenericDataFactory<EventIdData> _genericDataFactory;

        public EventIdDataFactory(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
            _genericDataFactory = new GenericDataFactory<EventIdData>();
        }

        public async Task<EventIdData> Get(ISqlSettings settings, Guid id)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "eventId", DbType.Guid, id)
            };

            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[bll].[GetEventId]",
                () => new EventIdData(),
                DataUtil.AssignDataStateManager,
                parameters))
                .FirstOrDefault();
        }

        public Task<IEnumerable<EventIdData>> GetByDomainId(ISqlSettings settings, Guid domainId)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "domainId", DbType.Guid, domainId)
            };

            return _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[bll].[GetEventId_by_DomainId]",
                () => new EventIdData(),
                DataUtil.AssignDataStateManager,
                parameters);
        }
    }
}
