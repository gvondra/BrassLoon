using BrassLoon.DataClient;
using BrassLoon.Log.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public interface IPurgeWorkerDataFactory
    {
        Task<Guid?> ClaimPurgeWorker(ISqlSettings settings);
        Task<IEnumerable<PurgeWorkerData>> GetAll(ISqlSettings settings);
        Task<PurgeWorkerData> Get(ISqlSettings settings, Guid id);
    }
}
