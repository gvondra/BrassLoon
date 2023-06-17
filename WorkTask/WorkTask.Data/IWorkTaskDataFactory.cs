﻿using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkTaskDataFactory
    {
        Task<WorkTaskData> Get(ISqlSettings settings, Guid id);
        Task<IEnumerable<WorkTaskData>> GetByWorkGroupId(ISqlSettings settings, Guid workGroupId, bool includeClosed = false);
        Task<IEnumerable<WorkTaskData>> GetByContextReference(ISqlSettings settings, short referenceType, byte[] referenceValueHash, bool includeClosed = false);
    }
}
