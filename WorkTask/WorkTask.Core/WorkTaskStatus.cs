using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Core;
using BrassLoon.WorkTask.Data;
using BrassLoon.WorkTask.Data.Models;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Framework
{
    public class WorkTaskStatus : IWorkTaskStatus
    {
        private readonly WorkTaskStatusData _data;
        private readonly IWorkTaskStatusDataSaver _dataSaver;
        private readonly IWorkTaskType _workTaskType;

        public WorkTaskStatus(WorkTaskStatusData data,
            IWorkTaskStatusDataSaver dataSaver,
            IWorkTaskType workTaskType)
        {
            _data = data;
            _dataSaver = dataSaver;
            _workTaskType = workTaskType;
        }

        public WorkTaskStatus(WorkTaskStatusData data,
            IWorkTaskStatusDataSaver dataSaver)
            : this(data, dataSaver, null) 
        {}

        public Guid WorkTaskTypeId { get => _data.WorkTaskTypeId; private set => _data.WorkTaskTypeId = value; }

        public Guid DomainId { get => _data.DomainId; private set => _data.DomainId = value; }

        public string Name { get => _data.Name; set => _data.Name = value; }
        public string Description { get => _data.Description; set => _data.Description = value; }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        public int WorkTaskCount => _data.WorkTaskCount;

        public Guid WorkTaskStatusId => _data.WorkTaskStatusId;

        public string Code => _data.Code;

        public bool IsDefaultStatus { get => _data.IsDefaultStatus; set => _data.IsDefaultStatus = value; }
        public bool IsClosedStatus { get => _data.IsClosedStatus; set => _data.IsClosedStatus = value; }

        public Task Create(ITransactionHandler transactionHandler)
        {
            if (_workTaskType == null)
                throw new ArgumentNullException(nameof(_workTaskType));
            WorkTaskTypeId = _workTaskType.WorkTaskTypeId;
            DomainId = _workTaskType.DomainId;
            return _dataSaver.Create(transactionHandler, _data);
        }

        public Task Update(ITransactionHandler transactionHandler) => _dataSaver.Update(transactionHandler, _data);        
    }
}
