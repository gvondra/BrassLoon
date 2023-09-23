using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Data;
using BrassLoon.WorkTask.Data.Models;
using BrassLoon.WorkTask.Framework;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public class PurgeWorkerFactory : IPurgeWorkerFactory
    {
        private readonly IPurgeWorkerDataFactory _dataFactory;
        private readonly IPurgeWorkerDataSaver _dataSaver;

        public PurgeWorkerFactory(
            IPurgeWorkerDataFactory dataFactory,
            IPurgeWorkerDataSaver dataSaver)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
        }

        public Task<Guid?> Claim(ISettings settings) => _dataFactory.ClaimPurgeWorker(new DataSettings(settings));

        public async Task<IPurgeWorker> Get(ISettings settings, Guid id)
        {
            IPurgeWorker result = null;
            PurgeWorkerData data = await _dataFactory.Get(new DataSettings(settings), id);
            if (data != null)
                result = new PurgeWorker(data, _dataSaver);
            return result;
        }
    }
}
