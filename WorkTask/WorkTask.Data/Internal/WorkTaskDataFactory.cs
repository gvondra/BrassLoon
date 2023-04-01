using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal
{
    public class WorkTaskDataFactory: DataFactoryBase<WorkTaskData>, IWorkTaskDataFactory
    {
        public WorkTaskDataFactory(IDbProviderFactory providerFactory) : base(providerFactory) { }

        public async Task<WorkTaskData> Get(ISqlSettings settings, Guid id)
        {
            WorkTaskData workTaskData = null;
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid, id)
            };
            DataReaderProcess dataReaderProcess = new DataReaderProcess();
            await dataReaderProcess.Read(settings, _providerFactory, "[blwt].[GetWorkTask]", CommandType.StoredProcedure,
                parameters: parameters,
                readAction: async (DbDataReader reader) =>
                {
                    workTaskData = (await _genericDataFactory.LoadData(reader, Create, DataUtil.AssignDataStateManager)).FirstOrDefault();
                    if (workTaskData != null)
                    {
                        GenericDataFactory<WorkTaskTypeData> typeFactory = new GenericDataFactory<WorkTaskTypeData>();
                        GenericDataFactory<WorkTaskStatusData> statusFactory = new GenericDataFactory<WorkTaskStatusData>();
                        GenericDataFactory<WorkTaskContextData> contextFactory = new GenericDataFactory<WorkTaskContextData>();
                        reader.NextResult();
                        workTaskData.WorkTaskType = (await typeFactory.LoadData(reader, () => new WorkTaskTypeData(), DataUtil.AssignDataStateManager)).FirstOrDefault();
                        reader.NextResult();
                        workTaskData.WorkTaskStatus = (await statusFactory.LoadData(reader, () => new WorkTaskStatusData(), DataUtil.AssignDataStateManager)).FirstOrDefault();
                        reader.NextResult();
                        workTaskData.WorkTaskContexts = (await contextFactory.LoadData(reader, () => new WorkTaskContextData(), DataUtil.AssignDataStateManager)).ToList();
                    }
                });
            return workTaskData;
        }

        public async Task<IEnumerable<WorkTaskData>> GetByWorkGroupId(ISqlSettings settings, Guid workGroupId, bool includeClosed = false)
        {
            IEnumerable<WorkTaskData> result = new List<WorkTaskData>();
            List<WorkTaskTypeData> types = null;
            List<WorkTaskStatusData> statuses = null;
            List<WorkTaskContextData> contexts = null;
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "workGroupId", DbType.Guid, workGroupId),
                DataUtil.CreateParameter(_providerFactory, "includeClosed", DbType.Boolean, includeClosed)
            };
            DataReaderProcess dataReaderProcess = new DataReaderProcess();
            await dataReaderProcess.Read(settings, _providerFactory, "[blwt].[GetWorkTask_by_WorkGroupId]", CommandType.StoredProcedure,
                parameters: parameters,
                readAction: async (DbDataReader reader) =>
                {
                    result = (await _genericDataFactory.LoadData(reader, Create, DataUtil.AssignDataStateManager)).ToList();
                    GenericDataFactory<WorkTaskTypeData> typeFactory = new GenericDataFactory<WorkTaskTypeData>();
                    GenericDataFactory<WorkTaskStatusData> statusFactory = new GenericDataFactory<WorkTaskStatusData>();
                    GenericDataFactory<WorkTaskContextData> contextFactory = new GenericDataFactory<WorkTaskContextData>();
                    reader.NextResult();
                    types = (await typeFactory.LoadData(reader, () => new WorkTaskTypeData(), DataUtil.AssignDataStateManager)).ToList();
                    reader.NextResult();
                    statuses = (await statusFactory.LoadData(reader, () => new WorkTaskStatusData(), DataUtil.AssignDataStateManager)).ToList();
                    reader.NextResult();
                    contexts = (await contextFactory.LoadData(reader, () => new WorkTaskContextData(), DataUtil.AssignDataStateManager)).ToList();                    
                });
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
            return result;
        }
    }
}
