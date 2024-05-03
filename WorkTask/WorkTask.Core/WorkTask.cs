using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Data;
using BrassLoon.WorkTask.Data.Models;
using BrassLoon.WorkTask.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public class WorkTask : IWorkTask, DataClient.IDbTransactionObserver
    {
        private readonly WorkTaskData _data;
        private readonly IWorkTaskDataSaver _dataSaver;
        private readonly WorkTaskFactory _factory;
        private readonly IWorkTaskType _workTaskType;
        private List<IWorkTaskContext> _contexts;
        private List<IWorkTaskContext> _newContexts;

        public WorkTask(
            WorkTaskData data,
            IWorkTaskDataSaver dataSaver,
            WorkTaskFactory factory,
            IWorkTaskType workTaskType,
            List<IWorkTaskContext> contexts = null)
        {
            _data = data;
            _dataSaver = dataSaver;
            _workTaskType = workTaskType;
            _factory = factory;
            _contexts = contexts ?? new List<IWorkTaskContext>();
        }

        public Guid WorkTaskId => _data.WorkTaskId;

        public Guid DomainId => _data.DomainId;

        public string Title { get => _data.Title; set => _data.Title = value; }
        public string Text { get => _data.Text; set => _data.Text = value ?? string.Empty; }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        public IWorkTaskType WorkTaskType => _workTaskType;

        public IWorkTaskStatus WorkTaskStatus { get; set; }

        public IReadOnlyList<IWorkTaskContext> WorkTaskContexts
            => ImmutableList<IWorkTaskContext>.Empty.AddRange(_contexts ?? new List<IWorkTaskContext>())
            .AddRange(_newContexts ?? new List<IWorkTaskContext>());

        public string AssignedToUserId { get => _data.AssignedToUserId; set => _data.AssignedToUserId = value; }
        public DateTime? AssignedDate { get => _data.AssignedDate; set => _data.AssignedDate = value; }
        public DateTime? ClosedDate { get => _data.ClosedDate; private set => _data.ClosedDate = value; }

        public async Task Create(ITransactionHandler transactionHandler)
        {
            if (_workTaskType == null)
                throw new ApplicationException("Unable to create work task as no work task type was specified");
            SetWorkTaskTypeId(_workTaskType.WorkTaskTypeId);
            SetWorkTaskStatusId();
            SetClosedDate();
            await _dataSaver.Create(transactionHandler, _data);
            await SaveNewContexts(transactionHandler);
        }

        public async Task Update(ITransactionHandler transactionHandler)
        {
            SetWorkTaskStatusId();
            SetClosedDate();
            await _dataSaver.Update(transactionHandler, _data);
            await SaveNewContexts(transactionHandler);
        }

        private void SetWorkTaskTypeId(Guid value) => _data.WorkTaskTypeId = value;

        private void SetWorkTaskStatusId()
        {
            if (WorkTaskStatus == null)
                throw new ApplicationException("Unable to create work task as no status has been set");
            _data.WorkTaskStatusId = WorkTaskStatus.WorkTaskStatusId;
        }

        private void SetClosedDate()
        {
            if (WorkTaskStatus == null)
                throw new ApplicationException("Unable to create work task as no status has been set");
            if (WorkTaskStatus.IsClosedStatus && !ClosedDate.HasValue)
                ClosedDate = DateTime.Today;
            else if (!WorkTaskStatus.IsClosedStatus && ClosedDate.HasValue)
                ClosedDate = null;
        }

        public IWorkTaskContext AddContext(short referenceType, string referenceValue)
        {
            if (referenceValue == null)
                referenceValue = string.Empty;
            IWorkTaskContext workTaskContext = null;
            if (!WorkTaskContexts.Any(c => referenceType == c.ReferenceType && referenceValue.Equals(c.ReferenceValue ?? string.Empty, StringComparison.OrdinalIgnoreCase)))
            {
                if (_newContexts == null)
                    _newContexts = new List<IWorkTaskContext>();
                workTaskContext = _factory.CreateContext(DomainId, this, referenceType, referenceValue);
                _newContexts.Add(workTaskContext);
            }
            return workTaskContext;
        }

        private async Task SaveNewContexts(ITransactionHandler transactionHandler)
        {
            if (_newContexts != null)
            {
                foreach (IWorkTaskContext context in _newContexts)
                {
                    await context.Create(transactionHandler);
                }
            }
        }

        void DataClient.IDbTransactionObserver.BeforeCommit() { }

        void DataClient.IDbTransactionObserver.AfterCommit() { }

        void DataClient.IDbTransactionObserver.BeforeRollback()
        {
            _contexts = (_contexts ?? new List<IWorkTaskContext>())
                .Concat(_newContexts ?? new List<IWorkTaskContext>())
                .ToList();
            _newContexts = null;
        }

        void DataClient.IDbTransactionObserver.AfterRollback() { }
    }
}
