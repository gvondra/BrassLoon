﻿using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal.SqlClient
{
    public class WorkTaskStatusDataSaver : DataSaverBase, IWorkTaskStatusDataSaver
    {
        public WorkTaskStatusDataSaver(IDbProviderFactory providerFactory)
            : base(providerFactory) { }

        public async Task Create(CommonData.ISaveSettings settings, WorkTaskStatusData data)
        {
            if (data.Manager.GetState(data) == DataState.New)
            {
                await ProviderFactory.EstablishTransaction(settings, data);
                using DbCommand command = settings.Connection.CreateCommand();
                command.CommandText = "[blwt].[CreateWorkTaskStatus]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = settings.Transaction.InnerTransaction;

                IDataParameter id = DataUtil.CreateParameter(ProviderFactory, "id", DbType.Guid);
                id.Direction = ParameterDirection.Output;
                _ = command.Parameters.Add(id);

                IDataParameter timestamp = DataUtil.CreateParameter(ProviderFactory, "timestamp", DbType.DateTime2);
                timestamp.Direction = ParameterDirection.Output;
                _ = command.Parameters.Add(timestamp);

                DataUtil.AddParameter(ProviderFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(data.DomainId));
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "code", DbType.AnsiString, DataUtil.GetParameterValue(data.Code));
                AddCommonParameters(command.Parameters, data);

                _ = await command.ExecuteNonQueryAsync();
                data.WorkTaskStatusId = (Guid)id.Value;
                data.CreateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
                data.UpdateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
            }
        }

        //public async Task Delete(CommonData.ISaveSettings settings, Guid id)
        //{
        //    await ProviderFactory.EstablishTransaction(settings);
        //    using DbCommand command = settings.Connection.CreateCommand();
        //    command.CommandText = "[blwt].[DeleteWorkTaskStatus]";
        //    command.CommandType = CommandType.StoredProcedure;
        //    command.Transaction = settings.Transaction.InnerTransaction;

        //    DataUtil.AddParameter(ProviderFactory, command.Parameters, "id", DbType.Guid, DataUtil.GetParameterValue(id));
        //    _ = await command.ExecuteNonQueryAsync();
        //}

        public async Task DeleteExcluding(CommonData.ISaveSettings settings, IEnumerable<Guid> ids)
        {
            await ProviderFactory.EstablishTransaction(settings);
            using DbCommand command = settings.Connection.CreateCommand();
            command.CommandText = "[blwt].[DeleteWorkTaskStatus_Excluding]";
            command.CommandType = CommandType.StoredProcedure;
            command.Transaction = settings.Transaction.InnerTransaction;

            DataUtil.AddParameter(
                ProviderFactory,
                command.Parameters,
                "ids",
                DbType.AnsiString,
                DataUtil.GetParameterValue(string.Join(",", ids.Select(i => i.ToString("D")))));
            _ = await command.ExecuteNonQueryAsync();
        }

        public async Task Update(CommonData.ISaveSettings settings, WorkTaskStatusData data)
        {
            if (data.Manager.GetState(data) == DataState.Updated)
            {
                await ProviderFactory.EstablishTransaction(settings, data);
                using DbCommand command = settings.Connection.CreateCommand();
                command.CommandText = "[blwt].[UpdateWorkTaskStatus]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = settings.Transaction.InnerTransaction;

                IDataParameter timestamp = DataUtil.CreateParameter(ProviderFactory, "timestamp", DbType.DateTime2);
                timestamp.Direction = ParameterDirection.Output;
                _ = command.Parameters.Add(timestamp);

                DataUtil.AddParameter(ProviderFactory, command.Parameters, "id", DbType.Guid, DataUtil.GetParameterValue(data.WorkTaskStatusId));
                AddCommonParameters(command.Parameters, data);

                _ = await command.ExecuteNonQueryAsync();
                data.UpdateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
            }
        }

        public async Task Save(CommonData.ISaveSettings settings, IEnumerable<WorkTaskStatusData> statuses)
        {
            statuses ??= Enumerable.Empty<WorkTaskStatusData>();
            foreach (WorkTaskStatusData data in statuses)
            {
                if (data.Manager.GetState(data) == DataState.New)
                {
                    await Create(settings, data);
                }
                else if (data.Manager.GetState(data) == DataState.Updated)
                {
                    await Update(settings, data);
                }
            }
            await DeleteExcluding(settings, statuses.Select(wts => wts.WorkTaskStatusId));
        }

        private void AddCommonParameters(IList commandParameters, WorkTaskStatusData data)
        {
            DataUtil.AddParameter(ProviderFactory, commandParameters, "workTaskTypeId", DbType.Guid, DataUtil.GetParameterValue(data.WorkTaskTypeId));
            DataUtil.AddParameter(ProviderFactory, commandParameters, "name", DbType.String, DataUtil.GetParameterValue(data.Name));
            DataUtil.AddParameter(ProviderFactory, commandParameters, "description", DbType.String, DataUtil.GetParameterValue(data.Description));
            DataUtil.AddParameter(ProviderFactory, commandParameters, "isDefaultStatus", DbType.Boolean, DataUtil.GetParameterValue(data.IsDefaultStatus));
            DataUtil.AddParameter(ProviderFactory, commandParameters, "isClosedStatus", DbType.Boolean, DataUtil.GetParameterValue(data.IsClosedStatus));
        }
    }
}
