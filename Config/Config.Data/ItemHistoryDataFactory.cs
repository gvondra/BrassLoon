using BrassLoon.Config.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BrassLoon.Config.Data
{
    public class ItemHistoryDataFactory : IItemHistoryDataFactory
    {
        private readonly ISqlDbProviderFactory _providerFactory;
        private readonly GenericDataFactory<ItemHistoryData> _genericDataFactory;

        public ItemHistoryDataFactory(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
            _genericDataFactory = new GenericDataFactory<ItemHistoryData>();
        }

        public async Task<IEnumerable<ItemHistoryData>> GetByItemId(ISqlSettings settings, Guid itemId)
        {
            List<IDataParameter> parameters = new List<IDataParameter>
            {
                DataUtil.CreateParameter(_providerFactory, "itemId", DbType.Guid, itemId)
            };
            return await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[blc].[GetItemHistoryByItemId]",
                () => new ItemHistoryData(),
                parameters);
        }
    }
}
