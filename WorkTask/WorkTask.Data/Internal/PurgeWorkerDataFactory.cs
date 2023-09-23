using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Data.Common;
using System.Data;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal
{
    public class PurgeWorkerDataFactory : DataFactoryBase<WorkTaskData>, IPurgeWorkerDataFactory
    {
        public PurgeWorkerDataFactory(IDbProviderFactory providerFactory) : base(providerFactory) { }

        public async Task<Guid?> ClaimPurgeWorker(ISqlSettings settings)
        {
            Guid? result = null;
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid);
            parameter.Direction = ParameterDirection.Output;

            using (DbConnection connection = await _providerFactory.OpenConnection(settings))
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[blwt].[ClaimPurgeWorker]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(parameter);
                    await command.ExecuteNonQueryAsync();
                    if (parameter.Value != null && parameter.Value != DBNull.Value)
                        result = (Guid)parameter.Value;
                }
            }
            return result;
        }
    }
}
