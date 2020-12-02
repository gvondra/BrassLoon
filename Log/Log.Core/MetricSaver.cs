using BrassLoon.CommonCore;
using BrassLoon.Log.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Core
{
    public class MetricSaver : IMetricSaver
    {
        public async Task Create(ISettings settings, params IMetric[] metrics)
        {
            if (metrics != null && metrics.Length > 0)
            {
                Saver saver = new Saver();
                await saver.Save(new TransactionHandler(settings), async th =>
                {
                    for (int i = 0; i < metrics.Length; i += 1)
                    {
                        await metrics[i].Create(th);
                    }
                });
            }
        }
    }
}
