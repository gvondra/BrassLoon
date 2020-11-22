using BrassLoon.DataClient;
using BrassLoon.Log.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public class MetricDataSaver : IMetricDataSaver
    {
        private IDbProviderFactory _providerFactory;

        public MetricDataSaver(IDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task Create(ITransactionHandler transactionHandler, MetricData metricData)
        {
            if (metricData.Manager.GetState(metricData) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, metricData);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[bll].[CreateMetric]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter id = DataUtil.CreateParameter(_providerFactory, "id", DbType.Int64);
                    id.Direction = ParameterDirection.Output;
                    command.Parameters.Add(id);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(metricData.DomainId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "eventCode", DbType.AnsiString, DataUtil.GetParameterValue(metricData.EventCode));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "magnitude", DbType.Double, DataUtil.GetParameterValue(metricData.Magnitude));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "data", DbType.String, DataUtil.GetParameterValue(metricData.Data));

                    await command.ExecuteNonQueryAsync();
                    metricData.MetricId = (long)id.Value;
                    metricData.CreateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }
    }
}
