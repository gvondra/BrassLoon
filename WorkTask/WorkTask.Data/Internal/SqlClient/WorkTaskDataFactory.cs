using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal.SqlClient
{
    public class WorkTaskDataFactory : DataFactoryBase<WorkTaskData>, IWorkTaskDataFactory
    {
        private readonly IWorkTaskTypeDataFactory _workTaskTypeDataFactory;
        private readonly ILoaderFactory _loaderFactory;

        public WorkTaskDataFactory(
            IDbProviderFactory providerFactory,
            IWorkTaskTypeDataFactory workTaskTypeDataFactory,
            ILoaderFactory loaderFactory)
            : base(providerFactory)
        {
            _workTaskTypeDataFactory = workTaskTypeDataFactory;
            _loaderFactory = loaderFactory;
        }

        public async Task<WorkTaskData> Get(CommonData.ISettings settings, Guid id)
        {
            WorkTaskData workTaskData = null;
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(ProviderFactory, "id", DbType.Guid, id)
            };
            DataReaderProcess dataReaderProcess = new DataReaderProcess();
            workTaskData = await dataReaderProcess.Read(
                settings,
                ProviderFactory,
                "[blwt].[GetWorkTask]",
                CommandType.StoredProcedure,
                parameters: parameters,
                readAction: async (reader) => (await ReadWorkTaskData(reader)).FirstOrDefault());
            return workTaskData;
        }

        public async Task<IEnumerable<WorkTaskData>> GetByWorkGroupId(CommonData.ISettings settings, Guid workGroupId, bool includeClosed = false)
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
                readAction: async (reader) =>
                {
                    result = await ReadWorkTaskData(reader);
                });
            return result;
        }

        public async Task<IEnumerable<WorkTaskData>> GetByContextReference(CommonData.ISettings settings, Guid domainId, short referenceType, byte[] referenceValueHash, bool includeClosed = false)
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
                readAction: async (reader) =>
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
            IEnumerable<WorkTaskTypeData> types = await typeFactory.LoadData(reader, () => new WorkTaskTypeData(), DataUtil.AssignDataStateManager);
            _ = await reader.NextResultAsync();
            List<WorkTaskStatusData> statuses = (await statusFactory.LoadData(reader, () => new WorkTaskStatusData(), DataUtil.AssignDataStateManager)).ToList();
            _ = await reader.NextResultAsync();
            List<WorkTaskContextData> contexts = (await contextFactory.LoadData(reader, () => new WorkTaskContextData(), DataUtil.AssignDataStateManager)).ToList();
            types = types.GroupJoin(
                statuses,
                typ => typ.WorkTaskTypeId,
                sts => sts.WorkTaskTypeId,
                (typ, sts) =>
                {
                    typ.Statuses = sts.ToList();
                    return typ;
                })
                .ToList();
            result = result.GroupJoin(contexts, tsk => tsk.WorkTaskId, ctx => ctx.WorkTaskId, (tsk, ctxs) =>
            {
                tsk.WorkTaskContexts = ctxs.ToList();
                return tsk;
            })
            .Join(types, tsk => tsk.WorkTaskTypeId, typ => typ.WorkTaskTypeId, (tsk, typ) =>
            {
                tsk.WorkTaskType = typ;
                tsk.WorkTaskStatus = typ.Statuses.Find(sts => sts.WorkTaskStatusId == tsk.WorkTaskStatusId);
                return tsk;
            });
            return result.ToList();
        }

        public async Task<IAsyncEnumerable<WorkTaskData>> GetAll(CommonData.ISettings settings, Guid domainId)
        {
            Dictionary<Guid, WorkTaskTypeData> workTaskTypes = (await _workTaskTypeDataFactory.GetByDomainId(settings, domainId))
                .ToDictionary(d => d.WorkTaskTypeId);
            return new WorkTaskDataEnumerable(
                settings,
                ProviderFactory,
                (connection) => GetAllBeginReader(connection, ProviderFactory, domainId),
                (reader) => GetAllLoadData(reader, settings, ProviderFactory, _loaderFactory.CreateLoader(), workTaskTypes));
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
            CommonData.ISettings settings,
            IDbProviderFactory providerFactory,
            ILoader loader,
            Dictionary<Guid, WorkTaskTypeData> workTaskTypes)
        {
            WorkTaskData workTaskData = (WorkTaskData)await loader.Load(new WorkTaskData(), reader);
            workTaskData.WorkTaskType = workTaskTypes[workTaskData.WorkTaskTypeId];
            workTaskData.WorkTaskStatus = workTaskData.WorkTaskType.Statuses.Find(sts => workTaskData.WorkTaskStatusId == sts.WorkTaskStatusId);
            workTaskData.WorkTaskContexts = (await GetContextByWorkTaskId(settings, providerFactory, workTaskData.WorkTaskId)).ToList();
            workTaskData.Manager = new DataStateManager(workTaskData.Clone());
            return workTaskData;
        }

        private static async Task<IEnumerable<WorkTaskContextData>> GetContextByWorkTaskId(CommonData.ISettings settings, IDbProviderFactory providerFactory, Guid workTaskId)
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
