using BrassLoon.CommonCore;
using BrassLoon.Log.Data;
using BrassLoon.Log.Data.Models;
using BrassLoon.Log.Framework;
using BrassLoon.Log.Framework.Enumerations;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Log.Core
{
    public class PurgeWorker : IPurgeWorker
    {
        private readonly PurgeWorkerData _data;
        private readonly IPurgeWorkerDataSaver _dataSaver;

        public PurgeWorker(PurgeWorkerData data,
            IPurgeWorkerDataSaver dataSaver)
        {
            _data = data;
            _dataSaver = dataSaver;
        }

        public Guid PurgeWorkerId => _data.PurgeWorkerId;

        public Guid DomainId => _data.DomainId;

        public PurgeWorkerStatus Status { get => (PurgeWorkerStatus)_data.Status; set => _data.Status = (short)value; }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        public async Task Update(ITransactionHandler transactionHandler)
        {
            await _dataSaver.Update(transactionHandler, _data);
        }
    }
}
