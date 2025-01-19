using BrassLoon.CommonData;
using BrassLoon.WorkTask.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkTaskStatusDataSaver
    {
        Task Save(ISaveSettings settings, IEnumerable<WorkTaskStatusData> statuses);
    }
}
