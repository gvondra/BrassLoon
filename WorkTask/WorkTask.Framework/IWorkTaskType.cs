using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public interface IWorkTaskType
    {
        Guid WorkTaskTypeId { get; }
        Guid DomainId { get; }
        string Title { get; set; }
        string Description { get; set; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }
        int WorkTaskCount { get; }

        Task Create(ITransactionHandler transactionHandler);
        Task Update(ITransactionHandler transactionHandler);
    }
}
