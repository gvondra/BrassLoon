using BrassLoon.Config.Data.Models;
using BrassLoon.Config.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace BrassLoon.Config.Core
{
    public class LookupHistory : ILookupHistory
    {
        private readonly LookupHistoryData _data;

        public LookupHistory(LookupHistoryData data)
        {
            _data = data;
        }

        public Guid LookupHistoryId => _data.LookupHistoryId;

        public Guid DomainId => _data.DomainId;

        public string Code => _data.Code;

#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
        public Dictionary<string, string> Data
        {
            get
            {
                if (string.IsNullOrEmpty(_data.Data))
                    return null;
                else
                    return JsonConvert.DeserializeObject<Dictionary<string, string>>(_data.Data, new JsonSerializerSettings() { ContractResolver = new DefaultContractResolver() });
            }
        }
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null

        public DateTime CreateTimestamp => _data.CreateTimestamp;
    }
}
