using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Data;
using BrassLoon.WorkTask.Data.Models;
using BrassLoon.WorkTask.Framework;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public class WorkGroup : IWorkGroup
    {
        private readonly WorkGroupData _data;
        private readonly IWorkGroupDataSaver _dataSaver;

        public WorkGroup(WorkGroupData data,
            IWorkGroupDataSaver dataSaver)
        {
            _data = data;
            _dataSaver = dataSaver;
        }

        public Guid WorkGroupId => _data.WorkGroupId;

        public Guid DomainId => _data.DomainId;

        public string Title { get => _data.Title; set => _data.Title = value ?? string.Empty; }
        public string Description { get => _data.Description; set => _data.Description = value ?? string.Empty; }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        public Task Create(ITransactionHandler transactionHandler) => _dataSaver.Create(transactionHandler, _data);

        public Task Update(ITransactionHandler transactionHandler) => _dataSaver.Update(transactionHandler, _data);
    }
}
