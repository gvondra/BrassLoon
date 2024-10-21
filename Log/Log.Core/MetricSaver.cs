using BrassLoon.CommonCore;
using BrassLoon.Log.Framework;
using System.Threading.Tasks;

namespace BrassLoon.Log.Core
{
    public class MetricSaver : IMetricSaver
    {
        public async Task Create(ISettings settings, params IMetric[] metrics)
        {
            if (metrics != null && metrics.Length > 0)
            {
                TransactionHandler transactionHandler = new TransactionHandler(settings);
                try
                {
                    for (int i = 0; i < metrics.Length; i += 1)
                    {
                        await metrics[i].Create(transactionHandler);
                        if (transactionHandler.Transaction != null)
                        {
                            transactionHandler.Transaction.Commit();
                            transactionHandler.Transaction.Dispose();
                            transactionHandler.Transaction = null;
                        }
                    }
                }
                catch
                {
                    if (transactionHandler.Transaction != null)
                    {
                        transactionHandler.Transaction.Rollback();
                        transactionHandler.Transaction.Dispose();
                        transactionHandler.Transaction = null;
                    }
                    throw;
                }
                finally
                {
                    if (transactionHandler.Connection != null)
                    {
                        await transactionHandler.Connection.DisposeAsync();
                        transactionHandler.Connection = null;
                    }
                }
            }
        }
    }
}
