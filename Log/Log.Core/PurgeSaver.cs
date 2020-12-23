using BrassLoon.CommonCore;
using BrassLoon.Log.Data;
using BrassLoon.Log.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Core
{
    public class PurgeSaver : IPurgeSaver
    {
        private readonly IPurgeDataSaver _dataSaver;
        private readonly SettingsFactory _settingsFactory;

        public PurgeSaver(IPurgeDataSaver dataSaver,
            SettingsFactory settingsFactory)
        {
            _dataSaver = dataSaver;
            _settingsFactory = settingsFactory;
        }

        public async Task Create(ISettings settings, params IPurgeMetaData[] purgeMetaData)
        {
            Saver saver = new Saver();
            await saver.Save(new TransactionHandler(settings), async (th) =>
            {
                if (purgeMetaData != null && purgeMetaData.Length > 0)
                {
                    for(int i = 0; i < purgeMetaData.Length; i += 1)
                    {
                        await purgeMetaData[i].Create(th);
                    }
                }
            });
        }

        public Task DeleteExceptionByMinTimestamp(ISettings settings, DateTime timestamp)
        {
            return _dataSaver.DeleteExceptionByMinTimestamp(_settingsFactory.CreateData(settings), timestamp);
        }

        public Task DeleteMetricByMinTimestamp(ISettings settings, DateTime timestamp)
        {
            return _dataSaver.DeleteMetricByMinTimestamp(_settingsFactory.CreateData(settings), timestamp);
        }

        public Task DeleteTraceByMinTimestamp(ISettings settings, DateTime timestamp)
        {
            return _dataSaver.DeleteTraceByMinTimestamp(_settingsFactory.CreateData(settings), timestamp);
        }

        public async Task Update(ISettings settings, params IPurgeMetaData[] purgeMetaData)
        {
            Saver saver = new Saver();
            await saver.Save(new TransactionHandler(settings), async (th) =>
            {
                if (purgeMetaData != null && purgeMetaData.Length > 0)
                {
                    for (int i = 0; i < purgeMetaData.Length; i += 1)
                    {
                        await purgeMetaData[i].Update(th);
                    }
                }
            });
        }
    }
}
