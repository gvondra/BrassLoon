using BrassLoon.Config.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.Config.Data.Internal.SqlClient
{
    public class ItemDataSaver : IItemDataSaver
    {
        private readonly ISqlDbProviderFactory _providerFactory;

        public ItemDataSaver(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task Create(CommonData.ISaveSettings saveSettings, ItemData itemData)
        {
            if (itemData.Manager.GetState(itemData) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(saveSettings, itemData);
                using (DbCommand command = saveSettings.Connection.CreateCommand())
                {
                    command.CommandText = "[blc].[CreateItem]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = saveSettings.Transaction.InnerTransaction;

                    IDataParameter id = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid);
                    id.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(id);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(itemData.DomainId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "code", DbType.AnsiString, DataUtil.GetParameterValue(itemData.Code));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "data", DbType.AnsiString, DataUtil.GetParameterValue(itemData.Data));

                    _ = await command.ExecuteNonQueryAsync();
                    itemData.ItemId = (Guid)id.Value;
                    itemData.CreateTimestamp = (DateTime)timestamp.Value;
                    itemData.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }

        public async Task DeleteByCode(CommonData.ISaveSettings saveSettings, Guid domainId, string code)
        {
            await _providerFactory.EstablishTransaction(saveSettings);
            using (DbCommand command = saveSettings.Connection.CreateCommand())
            {
                command.CommandText = "[blc].[DeleteItemByCode]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = saveSettings.Transaction.InnerTransaction;

                DataUtil.AddParameter(_providerFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(domainId));
                DataUtil.AddParameter(_providerFactory, command.Parameters, "code", DbType.AnsiString, DataUtil.GetParameterValue(code));

                _ = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task Update(CommonData.ISaveSettings saveSettings, ItemData itemData)
        {
            if (itemData.Manager.GetState(itemData) == DataState.Updated)
            {
                await _providerFactory.EstablishTransaction(saveSettings, itemData);
                using (DbCommand command = saveSettings.Connection.CreateCommand())
                {
                    command.CommandText = "[blc].[UpdateItem]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = saveSettings.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "id", DbType.Guid, DataUtil.GetParameterValue(itemData.ItemId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "code", DbType.AnsiString, DataUtil.GetParameterValue(itemData.Code));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "data", DbType.AnsiString, DataUtil.GetParameterValue(itemData.Data));

                    _ = await command.ExecuteNonQueryAsync();
                    itemData.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }
    }
}
