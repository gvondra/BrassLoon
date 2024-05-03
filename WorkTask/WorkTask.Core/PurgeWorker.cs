using System;
using System.Threading.Tasks;
using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Data;
using BrassLoon.WorkTask.Data.Models;
using BrassLoon.WorkTask.Framework;
using BrassLoon.WorkTask.Framework.Enumerations;

namespace BrassLoon.WorkTask.Core
{
    public class PurgeWorker : IPurgeWorker
    {
        private readonly PurgeWorkerData _data;
        private readonly IPurgeWorkerDataSaver _dataSaver;

        public PurgeWorker(
            PurgeWorkerData data,
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

        public Task Update(ITransactionHandler transactionHandler)
            => _dataSaver.Update(transactionHandler, _data);
    }
}
