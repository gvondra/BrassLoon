using BrassLoon.Config.Data;
using BrassLoon.Config.Data.Models;
using BrassLoon.Config.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Config.Core
{
    public class LookupHistoryFactory : ILookupHistoryFactory
    {
        private readonly ILookupHistoryDataFactory _dataFactory;
        private readonly SettingsFactory _settingsFactory;

        public LookupHistoryFactory(
            ILookupHistoryDataFactory dataFactory,
            SettingsFactory settingsFactory)
        {
            _dataFactory = dataFactory;
            _settingsFactory = settingsFactory;
        }

        public async Task<IEnumerable<ILookupHistory>> GetByLookupId(CommonCore.ISettings settings, Guid lookupId)
        {
            return (await _dataFactory.GetByLookupId(_settingsFactory.CreateDataSettings(settings), lookupId))
                .Select<LookupHistoryData, ILookupHistory>(data => new LookupHistory(data))
                ;
        }
    }
}
