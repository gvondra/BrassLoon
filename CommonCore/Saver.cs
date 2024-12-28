using System;
using System.Threading.Tasks;

namespace BrassLoon.CommonCore
{
    public static class Saver
    {
        public static async Task Save<T>(T transactionHandler, Func<T, Task> save)
            where T : DataClient.ISqlTransactionHandler
        {
            try
            {
                await save(transactionHandler);
                if (transactionHandler.Transaction != null)
                    transactionHandler.Transaction.Commit();
                if (transactionHandler.Connection != null)
                    await transactionHandler.Connection.CloseAsync();
            }
            catch
            {
                if (transactionHandler.Transaction != null)
                    transactionHandler.Transaction.Rollback();
                throw;
            }
            finally
            {
                if (transactionHandler.Transaction != null)
                {
                    transactionHandler.Transaction.Dispose();
                    transactionHandler.Transaction = null;
                }
                if (transactionHandler.Connection != null)
                {
                    await transactionHandler.Connection.DisposeAsync();
                    transactionHandler.Connection = null;
                }
            }
        }
    }
}
