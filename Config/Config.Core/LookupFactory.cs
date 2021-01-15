﻿using BrassLoon.CommonCore;
using BrassLoon.Config.Data;
using BrassLoon.Config.Data.Models;
using BrassLoon.Config.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Config.Core
{
    public class LookupFactory : ILookupFactory
    {
        private readonly ILookupDataFactory _dataFactory;
        private readonly ILookupDataSaver _dataSaver;
        private readonly SettingsFactory _settingsFactory;
        private readonly ILookupHistoryFactory _lookupHistoryFactory;

        public LookupFactory(ILookupDataFactory dataFactory,
            ILookupDataSaver dataSaver,
            SettingsFactory settingsFactory,
            ILookupHistoryFactory lookupHistoryFactory)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
            _settingsFactory = settingsFactory;
            _lookupHistoryFactory = lookupHistoryFactory;
        }

        public async Task<ILookup> GetByCode(ISettings settings, Guid domainId, string code)
        {
            Lookup result = null;
            LookupData data = await _dataFactory.GetByCode(_settingsFactory.CreateDataSettings(settings), domainId, code);
            if (data != null)
                result = new Lookup(data, _dataSaver, _lookupHistoryFactory);
            return result;
        }

        public Task<IEnumerable<string>> GetCodes(ISettings settings, Guid domainId)
        {
            return _dataFactory.GetCodes(_settingsFactory.CreateDataSettings(settings), domainId);
        }
    }
}
