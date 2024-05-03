using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal
{
    public class WorkTaskDataFactory : DataFactoryBase<WorkTaskData>, IWorkTaskDataFactory
    {
        private readonly IWorkTaskTypeDataFactory _workTaskTypeDataFactory;
        private readonly IWorkTaskStatusDataFactory _workTaskStatusDataFactory;
        private readonly ILoaderFactory _loaderFactory;

        public WorkTaskDataFactory(
            IDbProviderFactory providerFactory,
            IWorkTaskTypeDataFactory workTaskTypeDataFactory,
            IWorkTaskStatusDataFactory workTaskStatusDataFactory,
            ILoaderFactory loaderFactory)
            : base(providerFactory)
        {
            _workTaskTypeDataFactory = workTaskTypeDataFactory;
            _workTaskStatusDataFactory = workTaskStatusDataFactory;
            _loaderFactory = loaderFactory;
        }

        public async Task<WorkTaskData> Get(ISqlSettings settings, Guid id)
        {
            WorkTaskData workTaskData = null;
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(ProviderFactory, "id", DbType.Guid, id)
            };
            DataReaderProcess dataReaderProcess = new DataReaderProcess();
            await dataReaderProcess.Read(
                settings,
                ProviderFactory,
                "[blwt].[GetWorkTask]",
                CommandType.StoredProcedure,
                parameters: parameters,
                readAction: async (DbDataReader reader) =>
                {
                    workTaskData = (await GenericDataFactory.LoadData(reader, Create, DataUtil.AssignDataStateManager)).FirstOrDefault();
                    if (workTaskData != null)
                    {
                        GenericDataFactory<WorkTaskTypeData> typeFactory = new GenericDataFactory<WorkTaskTypeData>();
                        GenericDataFactory<WorkTaskStatusData> statusFactory = new GenericDataFactory<WorkTaskStatusData>();
                        GenericDataFactory<WorkTaskContextData> contextFactory = new GenericDataFactory<WorkTaskContextData>();
                        _ = reader.NextResult();
                        workTaskData.WorkTaskType = (await typeFactory.LoadData(reader, () => new WorkTaskTypeData(), DataUtil.AssignDataStateManager)).FirstOrDefault();
                        _ = reader.NextResult();
                        workTaskData.WorkTaskStatus = (await statusFactory.LoadData(reader, () => new WorkTaskStatusData(), DataUtil.AssignDataStateManager)).FirstOrDefault();
                        _ = reader.NextResult();
                        workTaskData.WorkTaskContexts = (await contextFactory.LoadData(reader, () => new WorkTaskContextData(), DataUtil.AssignDataStateManager)).ToList();
                    }
                });
            return workTaskData;
        }

        public async Task<IEnumerable<WorkTaskData>> GetByWorkGroupId(ISqlSettings settings, Guid workGroupId, bool includeClosed = false)
        {
            IEnumerable<WorkTaskData> result = new List<WorkTaskData>();
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(ProviderFactory, "workGroupId", DbType.Guid, workGroupId),
                DataUtil.CreateParameter(ProviderFactory, "includeClosed", DbType.Boolean, includeClosed)
            };
            DataReaderProcess dataReaderProcess = new DataReaderProcess();
            await dataReaderProcess.Read(
                settings,
                ProviderFactory,
                "[blwt].[GetWorkTask_by_WorkGroupId]",
                CommandType.StoredProcedure,
                parameters: parameters,
                readAction: async (DbDataReader reader) =>
                {
                    result = await ReadWorkTaskData(reader);
                });
            return result;
        }

        public async Task<IEnumerable<WorkTaskData>> GetByContextReference(ISqlSettings settings, Guid domainId, short referenceType, byte[] referenceValueHash, bool includeClosed = false)
        {
            IEnumerable<WorkTaskData> result = new List<WorkTaskData>();
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(ProviderFactory, "domainId", DbType.Guid, DataUtil.GetParameterValue(domainId)),
                DataUtil.CreateParameter(ProviderFactory, "referenceType", DbType.Int16, DataUtil.GetParameterValue(referenceType)),
                DataUtil.CreateParameter(ProviderFactory, "referenceValueHash", DbType.Binary, DataUtil.GetParameterValue(referenceValueHash)),
                DataUtil.CreateParameter(ProviderFactory, "includeClosed", DbType.Boolean, DataUtil.GetParameterValue(includeClosed))
            };
            DataReaderProcess dataReaderProcess = new DataReaderProcess();
            await dataReaderProcess.Read(
                settings,
                ProviderFactory,
                "[blwt].[GetWorkTask_by_ContextReference]",
                CommandType.StoredProcedure,
                parameters: parameters,
                readAction: async (DbDataReader reader) =>
                {
                    result = await ReadWorkTaskData(reader);
                });
            return result;
        }

        private static async Task<IEnumerable<WorkTaskData>> ReadWorkTaskData(DbDataReader reader)
        {
            GenericDataFactory<WorkTaskData> taskFactory = new GenericDataFactory<WorkTaskData>();
            GenericDataFactory<WorkTaskTypeData> typeFactory = new GenericDataFactory<WorkTaskTypeData>();
            GenericDataFactory<WorkTaskStatusData> statusFactory = new GenericDataFactory<WorkTaskStatusData>();
            GenericDataFactory<WorkTaskContextData> contextFactory = new GenericDataFactory<WorkTaskContextData>();
            IEnumerable<WorkTaskData> result = (await taskFactory.LoadData(reader, () => new WorkTaskData(), DataUtil.AssignDataStateManager)).ToList();
            _ = await reader.NextResultAsync();
            List<WorkTaskTypeData> types = (await typeFactory.LoadData(reader, () => new WorkTaskTypeData(), DataUtil.AssignDataStateManager)).ToList();
            _ = await reader.NextResultAsync();
            List<WorkTaskStatusData> statuses = (await statusFactory.LoadData(reader, () => new WorkTaskStatusData(), DataUtil.AssignDataStateManager)).ToList();
            _ = await reader.NextResultAsync();
            List<WorkTaskContextData> contexts = (await contextFactory.LoadData(reader, () => new WorkTaskContextData(), DataUtil.AssignDataStateManager)).ToList();
            result = result.GroupJoin(contexts, tsk => tsk.WorkTaskId, ctx => ctx.WorkTaskId, (tsk, ctxs) =>
            {
                tsk.WorkTaskContexts = ctxs.ToList();
                return tsk;
            })
            .Join(types, tsk => tsk.WorkTaskTypeId, typ => typ.WorkTaskTypeId, (tsk, typ) =>
            {
                tsk.WorkTaskType = typ;
                return tsk;
            })
            .Join(statuses, tsk => tsk.WorkTaskStatusId, sts => sts.WorkTaskStatusId, (tsk, sts) =>
            {
                tsk.WorkTaskStatus = sts;
                return tsk;
            });
            return result.ToList();
        }

        public async Task<IAsyncEnumerable<WorkTaskData>> GetAll(ISqlSettings settings, Guid domainId)
        {
            Dictionary<Guid, WorkTaskTypeData> workTaskTypes = (await _workTaskTypeDataFactory.GetByDomainId(settings, domainId))
                .ToDictionary(d => d.WorkTaskTypeId);
            Dictionary<Guid, WorkTaskStatusData> workTaskStatuses = (await _workTaskStatusDataFactory.GetByDomainId(settings, domainId))
                .ToDictionary(d => d.WorkTaskStatusId);
            return new WorkTaskDataEnumerable(
                settings,
                ProviderFactory,
                (connection) => GetAllBeginReader(connection, ProviderFactory, domainId),
                (reader) => GetAllLoadData(reader, settings, ProviderFactory, _loaderFactory.CreateLoader(), workTaskTypes, workTaskStatuses));
        }

        private static async Task<DbDataReader> GetAllBeginReader(DbConnection connection, IDbProviderFactory providerFactory, Guid domainId)
        {
            using DbCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[blwt].[GetWorkTask_by_DomainId]";
            _ = command.Parameters.Add(DataUtil.CreateParameter(providerFactory, "domainId", DbType.Guid, domainId));
            return await command.ExecuteReaderAsync();
        }

        private static async Task<WorkTaskData> GetAllLoadData(
            DbDataReader reader,
            ISqlSettings settings,
            IDbProviderFactory providerFactory,
            ILoader loader,
            Dictionary<Guid, WorkTaskTypeData> workTaskTypes,
            Dictionary<Guid, WorkTaskStatusData> workTaskStatuses)
        {
            WorkTaskData workTaskData = (WorkTaskData)await loader.Load(new WorkTaskData(), reader);
            workTaskData.WorkTaskType = workTaskTypes[workTaskData.WorkTaskTypeId];
            workTaskData.WorkTaskStatus = workTaskStatuses[workTaskData.WorkTaskStatusId];
            workTaskData.WorkTaskContexts = (await GetContextByWorkTaskId(settings, providerFactory, workTaskData.WorkTaskId)).ToList();
            workTaskData.Manager = new DataStateManager(workTaskData.Clone());
            return workTaskData;
        }

        private static async Task<IEnumerable<WorkTaskContextData>> GetContextByWorkTaskId(ISqlSettings settings, IDbProviderFactory providerFactory, Guid workTaskId)
        {
            GenericDataFactory<WorkTaskContextData> genericDataFactory = new GenericDataFactory<WorkTaskContextData>();
            IDataParameter parameter = DataUtil.CreateParameter(providerFactory, "workTaskId", DbType.Guid, workTaskId);
            return await genericDataFactory.GetData(
                settings,
                providerFactory,
                "[blwt].[GetWorkTaskContext_by_WorkTaskId]",
                () => new WorkTaskContextData(),
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter })
                ;
        }
    }
}
