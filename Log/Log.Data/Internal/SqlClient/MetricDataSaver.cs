using BrassLoon.DataClient;
using BrassLoon.Log.Data.Models;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data.Internal.SqlClient
{
    public class MetricDataSaver : IMetricDataSaver
    {
        private readonly ISqlDbProviderFactory _providerFactory;

        public MetricDataSaver(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task Create(CommonData.ISaveSettings settings, MetricData metricData)
        {
            if (metricData.Manager.GetState(metricData) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(settings, metricData);
                using (DbCommand command = settings.Connection.CreateCommand())
                {
                    command.CommandText = "[bll].[CreateMetric_v2]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = settings.Transaction.InnerTransaction;

                    IDataParameter id = DataUtil.CreateParameter(_providerFactory, "id", DbType.Int64);
                    id.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(id);

                    IDataParameter guid = DataUtil.CreateParameter(_providerFactory, "guid", DbType.Guid);
                    guid.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(guid);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(metricData.DomainId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "eventCode", DbType.AnsiString, DataUtil.GetParameterValue(metricData.EventCode));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "magnitude", DbType.Double, DataUtil.GetParameterValue(metricData.Magnitude));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "data", DbType.String, DataUtil.GetParameterValue(metricData.Data));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "timestamp", DbType.DateTime2, DataUtil.GetParameterValue(metricData.CreateTimestamp));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "status", DbType.AnsiString, DataUtil.GetParameterValue(metricData.Status));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "requestor", DbType.AnsiString, DataUtil.GetParameterValue(metricData.Requestor));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "eventId", DbType.Guid, DataUtil.GetParameterValue(metricData.EventId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "category", DbType.String, DataUtil.GetParameterValue(metricData.Category));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "level", DbType.String, DataUtil.GetParameterValue(metricData.Level));

                    _ = await command.ExecuteNonQueryAsync();
                    metricData.MetricId = (long)id.Value;
                    metricData.MetricGuid = (Guid)guid.Value;
                }
            }
        }
    }
}
