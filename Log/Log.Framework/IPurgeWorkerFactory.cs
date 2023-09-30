using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Log.Framework
{
    public interface IPurgeWorkerFactory
    {
        Task<IEnumerable<IPurgeWorker>> GetAll(ISettings settings);
        Task<IPurgeWorker> Get(ISettings settings, Guid id);
        Task<Guid?> Claim(ISettings settings);
    }
}
