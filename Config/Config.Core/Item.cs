using BrassLoon.CommonCore;
using BrassLoon.Config.Data;
using BrassLoon.Config.Data.Models;
using BrassLoon.Config.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Config.Core
{
    public class Item : IItem
    {
        private readonly ItemData _data;
        private readonly IItemDataSaver _dataSaver;
        private readonly IItemHistoryFactory _itemHistoryFactory;
        private List<IItemHistory> _itemHistories;

        public Item(ItemData data,
            IItemDataSaver dataSaver,
            IItemHistoryFactory itemHistoryFactory)
        {
            _data = data;
            _dataSaver = dataSaver;
            _itemHistoryFactory = itemHistoryFactory;
        }

        public Guid ItemId => _data.ItemId;

        public Guid DomainId => _data.DomainId;

        public string Code { get => _data.Code; set => _data.Code = value; }

        public dynamic Data
        {
            get
            {
                if (string.IsNullOrEmpty(_data.Data))
                    return null;
                else
                    return JsonConvert.DeserializeObject(_data.Data, new JsonSerializerSettings() { ContractResolver = new DefaultContractResolver() });
            }
            set
            {
                if (value == null)
                    _data.Data = null;
                else
                    _data.Data = JsonConvert.SerializeObject(value, new JsonSerializerSettings() { ContractResolver = new DefaultContractResolver() });
            }
        }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        public Task Create(ITransactionHandler transactionHandler)
        {
            return _dataSaver.Create(transactionHandler, _data);
        }

        public async Task<IEnumerable<IItemHistory>> GetHistory(ISettings settings)
        {
            if (_itemHistories == null)
                _itemHistories = (await _itemHistoryFactory.GetByItemId(settings, ItemId)).ToList();
            return _itemHistories;
        }

        public Task Update(ITransactionHandler transactionHandler)
        {
            return _dataSaver.Update(transactionHandler, _data);
        }
    }
}
