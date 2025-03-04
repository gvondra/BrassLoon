﻿using BrassLoon.CommonCore;
using BrassLoon.Log.Data;
using BrassLoon.Log.Data.Models;
using BrassLoon.Log.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Log.Core
{
    public class PurgeWorkerFactory : IPurgeWorkerFactory
    {
        private readonly IPurgeWorkerDataFactory _dataFactory;
        private readonly IPurgeWorkerDataSaver _dataSaver;
        private readonly SettingsFactory _settingsFactory;

        public PurgeWorkerFactory(
            IPurgeWorkerDataFactory dataFactory,
            IPurgeWorkerDataSaver dataSaver,
            SettingsFactory settingsFactory)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
            _settingsFactory = settingsFactory;
        }

        public Task<Guid?> Claim(ISettings settings) => _dataFactory.ClaimPurgeWorker(_settingsFactory.CreateData(settings));

        public async Task<IPurgeWorker> Get(ISettings settings, Guid id)
        {
            IPurgeWorker result = null;
            PurgeWorkerData data = await _dataFactory.Get(_settingsFactory.CreateData(settings), id);
            if (data != null)
                result = new PurgeWorker(data, _dataSaver);
            return result;
        }

        public async Task<IEnumerable<IPurgeWorker>> GetAll(ISettings settings)
        {
            return (await _dataFactory.GetAll(_settingsFactory.CreateData(settings)))
                .Select<PurgeWorkerData, IPurgeWorker>(data => new PurgeWorker(data, _dataSaver))
                ;
        }
    }
}
