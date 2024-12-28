using BrassLoon.Config.Data;
using BrassLoon.Config.Data.Models;
using BrassLoon.Config.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace BrassLoon.Config.Core
{
    public class LookupFactory : ILookupFactory
    {
        private readonly ILookupDataFactory _dataFactory;
        private readonly ILookupDataSaver _dataSaver;
        private readonly SettingsFactory _settingsFactory;
        private readonly ILookupHistoryFactory _lookupHistoryFactory;

        public LookupFactory(
            ILookupDataFactory dataFactory,
            ILookupDataSaver dataSaver,
            SettingsFactory settingsFactory,
            ILookupHistoryFactory lookupHistoryFactory)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
            _settingsFactory = settingsFactory;
            _lookupHistoryFactory = lookupHistoryFactory;
        }

        public ILookup Create(Guid domainId, string code) => new Lookup(new LookupData() { DomainId = domainId, Code = code.Trim().ToLower(CultureInfo.InvariantCulture) }, _dataSaver, _lookupHistoryFactory);

        public async Task<ILookup> GetByCode(CommonCore.ISettings settings, Guid domainId, string code)
        {
            Lookup result = null;
            LookupData data = await _dataFactory.GetByCode(_settingsFactory.CreateDataSettings(settings), domainId, code);
            if (data != null)
                result = new Lookup(data, _dataSaver, _lookupHistoryFactory);
            return result;
        }

        public Task<IEnumerable<string>> GetCodes(CommonCore.ISettings settings, Guid domainId) => _dataFactory.GetCodes(_settingsFactory.CreateDataSettings(settings), domainId);
    }
}
