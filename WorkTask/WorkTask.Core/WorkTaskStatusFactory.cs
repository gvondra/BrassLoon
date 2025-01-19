using BrassLoon.WorkTask.Data;
using BrassLoon.WorkTask.Data.Models;
using BrassLoon.WorkTask.Framework;
using System;

namespace BrassLoon.WorkTask.Core
{
    public class WorkTaskStatusFactory : IWorkTaskStatusFactory
    {
        private readonly IWorkTaskStatusDataSaver _dataSaver;

        public WorkTaskStatusFactory(IWorkTaskStatusDataSaver dataSaver)
        {
            _dataSaver = dataSaver;
        }

        internal WorkTaskStatus Create(WorkTaskStatusData data) => new WorkTaskStatus(data, _dataSaver);
        internal WorkTaskStatus Create(WorkTaskStatusData data, IWorkTaskType workTaskType) => new WorkTaskStatus(data, _dataSaver, workTaskType);

        public IWorkTaskStatus Create(IWorkTaskType workTaskType, string code)
        {
            ArgumentNullException.ThrowIfNull(workTaskType);
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException(nameof(code));
            return Create(
                new WorkTaskStatusData { Code = code, DomainId = workTaskType.DomainId },
                workTaskType);
        }
    }
}
