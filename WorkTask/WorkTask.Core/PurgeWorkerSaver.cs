using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Data;
using BrassLoon.WorkTask.Framework;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public class PurgeWorkerSaver : IPurgeWorkerSaver
    {
        private readonly IPurgeWorkerDataSaver _dataSaver;

        public PurgeWorkerSaver(IPurgeWorkerDataSaver dataSaver)
        {
            _dataSaver = dataSaver;
        }

        public Task InitializePurgeWorker(ISettings settings)
            => _dataSaver.InitializePurgeWorker(new DataSettings(settings));

        public async Task Update(ISettings settings, params IPurgeWorker[] purgeWorker)
        {
            if (purgeWorker != null)
            {
                await Saver.Save(new SaveSettings(settings), async ss =>
                {
                    for (int i = 0; i < purgeWorker.Length; i += 1)
                    {
                        await purgeWorker[i].Update(ss);
                    }
                });
            }
        }
    }
}
