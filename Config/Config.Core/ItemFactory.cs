using BrassLoon.CommonCore;
using BrassLoon.Config.Data;
using BrassLoon.Config.Data.Models;
using BrassLoon.Config.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Config.Core
{
    public class ItemFactory : IItemFactory
    {
        private readonly IItemDataFactory _dataFactory;
        private readonly IItemDataSaver _dataSaver;
        private readonly SettingsFactory _settingsFactory;
        private readonly IItemHistoryFactory _itemHistoryFactory;

        public ItemFactory(IItemDataFactory dataFactory,
            IItemDataSaver dataSaver,
            SettingsFactory settingsFactory,
            IItemHistoryFactory itemHistoryFactory)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
            _settingsFactory = settingsFactory;
            _itemHistoryFactory = itemHistoryFactory;
        }

        public IItem Create(Guid domainId, string code)
        {
            return new Item(new ItemData() { DomainId = domainId, Code = code }, _dataSaver, _itemHistoryFactory);
        }

        public async Task<IItem> GetByCode(ISettings settings, Guid domainId, string code)
        {
            Item result = null;
            ItemData data = await _dataFactory.GetByCode(_settingsFactory.CreateDataSettings(settings), domainId, code);
            if (data != null)
                result = new Item(data, _dataSaver, _itemHistoryFactory);
            return result;
        }

        public Task<IEnumerable<string>> GetCodes(ISettings settings, Guid domainId)
        {
            return _dataFactory.GetCodes(_settingsFactory.CreateDataSettings(settings), domainId);
        }
    }
}
