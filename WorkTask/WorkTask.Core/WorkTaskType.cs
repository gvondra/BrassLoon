using BrassLoon.CommonCore;
using BrassLoon.DataClient;
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
    public class WorkTaskType : IWorkTaskType, IDbTransactionObserver
    {
        private readonly WorkTaskTypeData _data;
        private readonly IWorkTaskTypeDataSaver _dataSaver;
        private readonly IWorkTaskTypeFactory _factory;
        private ImmutableList<IWorkTaskStatus> _statuses;
        private List<IWorkTaskStatus> _addedStatues;
        private List<IWorkTaskStatus> _removedStatues;

        public WorkTaskType(
            WorkTaskTypeData data,
            IWorkTaskTypeDataSaver dataSaver,
            IWorkTaskTypeFactory factory)
        {
            _data = data;
            _dataSaver = dataSaver;
            _factory = factory;
            _statuses = data.Statuses != null
                ? ImmutableList.CreateRange(data.Statuses.Select<WorkTaskStatusData, IWorkTaskStatus>(d => new WorkTaskStatus(d)))
                : ImmutableList<IWorkTaskStatus>.Empty;
        }

        public Guid WorkTaskTypeId => _data.WorkTaskTypeId;

        public Guid DomainId => _data.DomainId;

        public string Code => _data.Code;

        public string Title { get => _data.Title; set => _data.Title = value; }
        public string Description { get => _data.Description; set => _data.Description = value; }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        public int WorkTaskCount => _data.WorkTaskCount;

        public short? PurgePeriod { get => _data.PurgePeriod; set => _data.PurgePeriod = value > 0 ? value : default; }

        public IEnumerable<IWorkTaskStatus> Statuses
            => _statuses.Where(sts => _removedStatues == null || !_removedStatues.Exists(rmsts => rmsts == sts)).Concat(_addedStatues ?? Enumerable.Empty<IWorkTaskStatus>());

        public void AddWorkTaskStatus(IWorkTaskStatus workTaskStatus)
        {
            _addedStatues ??= new List<IWorkTaskStatus>();
            if (!_addedStatues.Contains(workTaskStatus))
                _addedStatues.Add(workTaskStatus);
        }

        public void AfterCommit()
        {
            _statuses = _data.Statuses != null
               ? ImmutableList.CreateRange(_data.Statuses.Select<WorkTaskStatusData, IWorkTaskStatus>(d => new WorkTaskStatus(d)))
               : ImmutableList<IWorkTaskStatus>.Empty;
            _addedStatues = null;
            _removedStatues = null;
        }

        public void AfterRollback() { }
        public void BeforeCommit() { }
        public void BeforeRollback() { }

        public async Task Create(ISaveSettings settings)
        {
            _data.Statuses = Statuses.Where(sts => sts is WorkTaskStatus).Select(sts => ((WorkTaskStatus)sts).InnerData).ToList();
            await _dataSaver.Create(settings, _data);
            settings.Transaction?.AddObserver(this);
        }

        public IWorkTaskStatus CreateWorkTaskStatus(string code)
            => _factory.GetWorkTaskStatusFactory().Create(this, code);

        public void RemoveWorkTaskStatus(IWorkTaskStatus workTaskStatus)
        {
            _removedStatues ??= new List<IWorkTaskStatus>();
            if (!_removedStatues.Contains(workTaskStatus))
                _removedStatues.Add(workTaskStatus);
        }

        public async Task Update(ISaveSettings settings)
        {
            _data.Statuses = Statuses.Where(sts => sts is WorkTaskStatus).Select(sts => ((WorkTaskStatus)sts).InnerData).ToList();
            await _dataSaver.Update(settings, _data);
            settings.Transaction?.AddObserver(this);
        }
    }
}
