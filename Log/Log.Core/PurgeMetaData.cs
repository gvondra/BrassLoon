using BrassLoon.CommonCore;
using BrassLoon.Log.Data;
using BrassLoon.Log.Data.Models;
using BrassLoon.Log.Framework;
using BrassLoon.Log.Framework.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Core
{
    public class PurgeMetaData : IPurgeMetaData
    {
        private readonly PurgeData _data;
        private readonly IPurgeDataSaver _dataSaver;
        private readonly Func<IPurgeDataSaver, ITransactionHandler, PurgeData, Task> _createDelegate;
        private readonly Func<IPurgeDataSaver, ITransactionHandler, PurgeData, Task> _updateDelegate;

        public PurgeMetaData(PurgeData data,
            IPurgeDataSaver dataSaver,
            Func<IPurgeDataSaver, ITransactionHandler, PurgeData, Task> createDelegate,
            Func<IPurgeDataSaver, ITransactionHandler, PurgeData, Task> updateDelegate)
        {
            _data = data;
            _dataSaver = dataSaver;
            _createDelegate = createDelegate;
            _updateDelegate = updateDelegate;
        }

        public long PurgeId => _data.PurgeId;

        public Guid DomainId => _data.DomainId;

        public PurgeMetaDataStatus Status { get => (PurgeMetaDataStatus)_data.Status; set => _data.Status = (short)value; }

        public long TargetId => _data.TargetId;

        public DateTime ExpirationTimestamp { get => _data.ExpirationTimestamp; set => _data.ExpirationTimestamp = value; }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        public Task Create(ITransactionHandler transactionHandler)
        {
            return _createDelegate(_dataSaver, transactionHandler, _data);
        }

        public Task Update(ITransactionHandler transactionHandler)
        {
            return _updateDelegate(_dataSaver, transactionHandler, _data);
        }
    }
}
