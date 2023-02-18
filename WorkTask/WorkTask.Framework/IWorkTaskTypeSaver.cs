using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public interface IWorkTaskTypeSaver
    {
        Task Create(ISettings settings, params IWorkTaskStatus[] statuses);
        Task Update(ISettings settings, params IWorkTaskStatus[] statuses);
        Task Create(ISettings settings, params IWorkTaskType[] types);
        Task Update(ISettings settings, params IWorkTaskType[] types);
    }
}
