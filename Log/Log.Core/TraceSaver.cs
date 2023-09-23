using BrassLoon.CommonCore;
using BrassLoon.Log.Framework;
using System.Threading.Tasks;

namespace BrassLoon.Log.Core
{
    public class TraceSaver : ITraceSaver
    {
        public async Task Create(ISettings settings, params ITrace[] traces)
        {
            if (traces != null && traces.Length > 0)
            {
                TransactionHandler transactionHandler = new TransactionHandler(settings);
                try
                {
                    for (int i = 0; i < traces.Length; i += 1)
                    {
                        await traces[i].Create(transactionHandler);
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
                        transactionHandler.Connection.Dispose();
                        transactionHandler.Connection = null;
                    }
                }
            }            
        }
    }
}
