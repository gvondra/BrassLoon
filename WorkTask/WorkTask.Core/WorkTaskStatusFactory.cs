using BrassLoon.WorkTask.Data.Models;
using BrassLoon.WorkTask.Framework;
using System;

namespace BrassLoon.WorkTask.Core
{
    public class WorkTaskStatusFactory : IWorkTaskStatusFactory
    {
        public IWorkTaskStatus Create(IWorkTaskType workTaskType, string code)
        {
            ArgumentNullException.ThrowIfNull(workTaskType);
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException(nameof(code));
            return Create(
                new WorkTaskStatusData { Code = code, DomainId = workTaskType.DomainId });
        }

        internal static WorkTaskStatus Create(WorkTaskStatusData data) => new WorkTaskStatus(data);
    }
}
