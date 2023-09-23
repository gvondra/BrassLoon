using BrassLoon.CommonCore;
using System.Threading.Tasks;

namespace BrassLoon.Log.Framework
{
    public interface IPurgeWorkerSaver
    {
        Task InitializePurgeWorker(ISettings settings);
        Task Update(ISettings settings, params IPurgeWorker[] purgeWorker);
    }
}
