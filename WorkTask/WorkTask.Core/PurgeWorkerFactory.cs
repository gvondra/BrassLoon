using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Data;
using BrassLoon.WorkTask.Framework;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public class PurgeWorkerFactory : IPurgeWorkerFactory
    {
        private readonly IPurgeWorkerDataFactory _dataFactory;

        public Task<Guid?> Claim(ISettings settings) => _dataFactory.ClaimPurgeWorker(new DataSettings(settings));
    }
}
