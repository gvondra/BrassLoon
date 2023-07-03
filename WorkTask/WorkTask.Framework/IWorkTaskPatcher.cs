using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Framework
{
    public interface IWorkTaskPatcher
    {
        Task<IEnumerable<IWorkTask>> Apply(ISettings settings, Guid domainId, IEnumerable<Dictionary<string, object>> patchData);
    }
}
