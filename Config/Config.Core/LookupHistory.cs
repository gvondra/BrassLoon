using BrassLoon.Config.Data.Models;
using BrassLoon.Config.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

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
