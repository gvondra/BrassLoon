using BrassLoon.CommonCore;
using BrassLoon.Log.Data;
using BrassLoon.Log.Framework;
using System.Threading.Tasks;

namespace BrassLoon.Log.Core
{
    public class PurgeWorkerSaver : IPurgeWorkerSaver
    {
        private readonly IPurgeWorkerDataSaver _dataSaver;
        private readonly SettingsFactory _settingsFactory;

        public PurgeWorkerSaver(
            IPurgeWorkerDataSaver dataSaver,
            SettingsFactory settingsFactory)
        {
            _dataSaver = dataSaver;
            _settingsFactory = settingsFactory;
        }

        public Task InitializePurgeWorker(ISettings settings) => _dataSaver.InitializePurgeWorker(_settingsFactory.CreateData(settings));

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
