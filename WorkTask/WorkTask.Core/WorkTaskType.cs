using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Data;
using BrassLoon.WorkTask.Data.Models;
using BrassLoon.WorkTask.Framework;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public class WorkTaskType : IWorkTaskType
    {
        private readonly WorkTaskTypeData _data;
        private readonly IWorkTaskTypeDataSaver _dataSaver;
        private readonly IWorkTaskTypeFactory _factory;

        public WorkTaskType(
            WorkTaskTypeData data,
            IWorkTaskTypeDataSaver dataSaver,
            IWorkTaskTypeFactory factory)
        {
            _data = data;
            _dataSaver = dataSaver;
            _factory = factory;
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

        public Task Create(ITransactionHandler transactionHandler) => _dataSaver.Create(transactionHandler, _data);

        public IWorkTaskStatus CreateWorkTaskStatus(string code)
            => _factory.GetWorkTaskStatusFactory().Create(this, code);

        public Task Update(ITransactionHandler transactionHandler) => _dataSaver.Update(transactionHandler, _data);
    }
}
