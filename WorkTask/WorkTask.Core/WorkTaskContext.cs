using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Data;
using BrassLoon.WorkTask.Data.Models;
using BrassLoon.WorkTask.Framework;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public class WorkTaskContext : IWorkTaskContext
    {
        private readonly WorkTaskContextData _data;
        private readonly IWorkTaskContextDataSaver _dataSaver;
        private readonly IWorkTask _workTask;

        public WorkTaskContext(WorkTaskContextData data,
            IWorkTaskContextDataSaver dataSaver,
            IWorkTask workTask)
        {
            _data = data;
            _dataSaver = dataSaver;
            _workTask = workTask;
        }

        public WorkTaskContext(WorkTaskContextData data, IWorkTaskContextDataSaver dataSaver)
            : this(data, dataSaver, null)
        { }

        public Guid WorkTaskContextId => _data.WorkTaskContextId;

        public Guid DomainId => _data.DomainId;

        public Guid WorkTaskId { get => _data.WorkTaskId; private set => _data.WorkTaskId = value; }

        public short ReferenceType => _data.ReferenceType;

        public string ReferenceValue => _data.ReferenceValue;

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public Task Create(ITransactionHandler transactionHandler)
        {
            if (_workTask == null)
            {
                throw new ApplicationException("A parent work task must be specified when creating a new work task context");
                // Make sure to use the contructor that takes an IWorkTask when creating new work task contexts
            }
            WorkTaskId = _workTask.WorkTaskId;
            return _dataSaver.Create(transactionHandler, _data);
        }
    }
}
