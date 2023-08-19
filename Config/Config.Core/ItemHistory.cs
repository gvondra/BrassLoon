using BrassLoon.Config.Data.Models;
using BrassLoon.Config.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace BrassLoon.Config.Core
{
    public class ItemHistory : IItemHistory
    {
        private readonly ItemHistoryData _data;

        public ItemHistory(ItemHistoryData data)
        {
            _data = data;
        }

        public Guid ItemHistoryId => _data.ItemHistoryId;

        public Guid DomainId => _data.DomainId;

        public string Code => _data.Code;

        public dynamic Data
        {
            get
            {
                if (string.IsNullOrEmpty(_data.Data))
                    return null;
                else
                    return JsonConvert.DeserializeObject(_data.Data, new JsonSerializerSettings() { ContractResolver = new DefaultContractResolver() });
            }
        }

        public DateTime CreateTimestamp => _data.CreateTimestamp;
    }
}
