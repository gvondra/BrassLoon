using BrassLoon.CommonCore;
using BrassLoon.Log.Data;
using BrassLoon.Log.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Core
{
    public class PurgeWorkerSaver : IPurgeWorkerSaver
    {
        private readonly IPurgeWorkerDataSaver _dataSaver;
        private readonly SettingsFactory _settingsFactory;
        
        public PurgeWorkerSaver(IPurgeWorkerDataSaver dataSaver,
            SettingsFactory settingsFactory)
        {
            _dataSaver = dataSaver;
            _settingsFactory = settingsFactory;
        }

        public Task InitializePurgeWorker(ISettings settings)
        {
            return _dataSaver.InitializePurgeWorker(_settingsFactory.CreateData(settings));
        }

        public async Task Update(ISettings settings, params IPurgeWorker[] purgeWorker)
        {
            if (purgeWorker != null)
            {
                Saver saver = new Saver();
                await saver.Save(new TransactionHandler(settings), async th =>
                {
                    for (int i = 0; i < purgeWorker.Length; i += 1)
                    {
                        await purgeWorker[i].Update(th);
                    }
                });
            }            
        }
    }
}
