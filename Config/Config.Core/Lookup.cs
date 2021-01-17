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
    public class Lookup : ILookup
    {
        private readonly LookupData _data;
        private readonly ILookupDataSaver _dataSaver;
        private readonly ILookupHistoryFactory _lookupHistoryFactory;
        private List<ILookupHistory> _lookupHistories;

        public Lookup(LookupData data,
            ILookupDataSaver dataSaver,
            ILookupHistoryFactory lookupHistoryFactory)
        {
            _data = data;
            _dataSaver = dataSaver;
            _lookupHistoryFactory = lookupHistoryFactory;
        }

        public Guid LookupId => _data.LookupId;

        public Guid DomainId => _data.DomainId;

        public string Code { get => _data.Code; set => _data.Code = value; }

        public Dictionary<string, string> Data 
        {
            get
            {
                if (string.IsNullOrEmpty(_data.Data))
                    return null;
                else
                    return JsonConvert.DeserializeObject<Dictionary<string, string>>(_data.Data, new JsonSerializerSettings() { ContractResolver = new DefaultContractResolver() });
            }
            set
            {
                if (value == null)
                    _data.Data = string.Empty;
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

        public async Task<IEnumerable<ILookupHistory>> GetHistory(ISettings settings)
        {
            if (_lookupHistories == null)
                _lookupHistories = (await _lookupHistoryFactory.GetByLookupId(settings, LookupId)).ToList();
            return _lookupHistories;
        }

        public Task Update(ITransactionHandler transactionHandler)
        {
            return _dataSaver.Update(transactionHandler, _data);
        }
    }
}
