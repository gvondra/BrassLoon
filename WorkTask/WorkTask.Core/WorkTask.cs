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
    public class WorkTask : IWorkTask, BrassLoon.DataClient.IDbTransactionObserver
    {
        private readonly WorkTaskData _data;
        private readonly IWorkTaskDataSaver _dataSaver;
        private readonly WorkTaskFactory _factory;
        private readonly IWorkTaskType _workTaskType;
        private IWorkTaskStatus _workTaskStatus;
        private List<IWorkTaskContext> _contexts;
        private List<IWorkTaskContext> _newContexts;

        public WorkTask(WorkTaskData data, 
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

        private Guid WorkTaskTypeId { get => _data.WorkTaskTypeId; set => _data.WorkTaskTypeId = value; }
        public IWorkTaskType WorkTaskType => _workTaskType;

        private Guid WorkTaskStatusId { get => _data.WorkTaskStatusId; set => _data.WorkTaskStatusId = value; }
        public IWorkTaskStatus WorkTaskStatus { get => _workTaskStatus; set => _workTaskStatus = value; }

        public IReadOnlyList<IWorkTaskContext> WorkTaskContexts 
            => ImmutableList<IWorkTaskContext>.Empty.AddRange(_contexts ?? new List<IWorkTaskContext>())
            .AddRange(_newContexts ?? new List<IWorkTaskContext>());

        public async Task Create(ITransactionHandler transactionHandler)
        {
            if (_workTaskType == null)
                throw new ApplicationException("Unable to create work task as no work task type was specified");            
            WorkTaskTypeId = _workTaskType.WorkTaskTypeId;
            SetWorkTaskStatusId();
            await _dataSaver.Create(transactionHandler, _data);
            await SaveNewContexts(transactionHandler);
        }

        public async Task Update(ITransactionHandler transactionHandler)
        {
            SetWorkTaskStatusId();
            await _dataSaver.Update(transactionHandler, _data);
            await SaveNewContexts(transactionHandler);
        }

        private void SetWorkTaskStatusId()
        {
            if (_workTaskStatus == null)
                throw new ApplicationException("Unable to create work task as no status has been set");
            WorkTaskStatusId = _workTaskStatus.WorkTaskStatusId;
        }

        public IWorkTaskContext AddContext(short referenceType, string referenceValue)
        {
            if (_newContexts == null)
                _newContexts = new List<IWorkTaskContext>();
            IWorkTaskContext context = _factory.CreateContext(DomainId, this, referenceType, referenceValue);
            _newContexts.Add(context);
            return context;
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

        void DataClient.IDbTransactionObserver.BeforeCommit() {}

        void DataClient.IDbTransactionObserver.AfterCommit() {}

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
