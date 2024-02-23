using BrassLoon.CommonCore;
using BrassLoon.Config.Data;
using BrassLoon.Config.Data.Models;
using BrassLoon.Config.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Config.Core
{
    public class ItemHistoryFactory : IItemHistoryFactory
    {
        private readonly IItemHistoryDataFactory _dataFactory;
        private readonly SettingsFactory _settingsFactory;

        public ItemHistoryFactory(
            IItemHistoryDataFactory dataFactory,
            SettingsFactory settingsFactory)
        {
            _dataFactory = dataFactory;
            _settingsFactory = settingsFactory;
        }

        public async Task<IEnumerable<IItemHistory>> GetByItemId(ISettings settings, Guid itemId)
        {
            return (await _dataFactory.GetByItemId(_settingsFactory.CreateDataSettings(settings), itemId))
                .Select<ItemHistoryData, IItemHistory>(data => new ItemHistory(data));
        }
    }
}
