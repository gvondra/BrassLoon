using BrassLoon.CommonData;
using BrassLoon.Log.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public interface IPurgeWorkerDataFactory
    {
        Task<Guid?> ClaimPurgeWorker(ISettings settings);
        Task<IEnumerable<PurgeWorkerData>> GetAll(ISettings settings);
        Task<PurgeWorkerData> Get(ISettings settings, Guid id);
    }
}
