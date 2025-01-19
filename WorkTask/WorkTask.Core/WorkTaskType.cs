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
    public class WorkTaskType : IWorkTaskType
    {
        private readonly WorkTaskTypeData _data;
        private readonly IWorkTaskTypeDataSaver _dataSaver;
        private readonly IWorkTaskTypeFactory _factory;
        private readonly ImmutableList<IWorkTaskStatus> _statuses;

        public WorkTaskType(
            WorkTaskTypeData data,
            IWorkTaskTypeDataSaver dataSaver,
            IWorkTaskStatusDataSaver statusDataSaver,
            IWorkTaskTypeFactory factory)
        {
            _data = data;
            _dataSaver = dataSaver;
            _factory = factory;
            _statuses = data.Statuses != null
                ? ImmutableList.CreateRange(data.Statuses.Select<WorkTaskStatusData, IWorkTaskStatus>(d => new WorkTaskStatus(d, statusDataSaver)))
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

        public IEnumerable<IWorkTaskStatus> Statuses => _statuses;

        public Task Create(ISaveSettings settings) => _dataSaver.Create(settings, _data);

        public IWorkTaskStatus CreateWorkTaskStatus(string code)
            => _factory.GetWorkTaskStatusFactory().Create(this, code);

        public Task Update(ISaveSettings settings) => _dataSaver.Update(settings, _data);
    }
}
