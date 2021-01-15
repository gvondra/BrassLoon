using BrassLoon.Config.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Config.Data
{
    public class LookupHistoryDataFactory : ILookupHistoryDataFactory
    {
        private ISqlDbProviderFactory _providerFactory;
        private GenericDataFactory<LookupHistoryData> _genericDataFactory;

        public LookupHistoryDataFactory(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
            _genericDataFactory = new GenericDataFactory<LookupHistoryData>();
        }

        public async Task<IEnumerable<LookupHistoryData>> GetByLookupId(ISqlSettings settings, Guid lookupId)
        {
            List<IDataParameter> parameters = new List<IDataParameter>
            {
                DataUtil.CreateParameter(_providerFactory, "lookupId", DbType.Guid, lookupId)
            };
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[blc].[GetLookupHistoryByLookupId]",
                () => new LookupHistoryData(),
                parameters
                ));
        }
    }
}
