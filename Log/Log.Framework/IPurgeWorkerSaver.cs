using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Framework
{
    public interface IPurgeWorkerSaver
    {
        Task InitializePurgeWorker(ISettings settings);
        Task Update(ISettings settings, params IPurgeWorker[] purgeWorker);
    }
}
