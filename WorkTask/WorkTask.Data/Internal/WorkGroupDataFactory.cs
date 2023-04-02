using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal
{
    public class WorkGroupDataFactory : DataFactoryBase<WorkGroupData>, IWorkGroupDataFactory
    {
        public WorkGroupDataFactory(IDbProviderFactory providerFactory) : base(providerFactory) { }

        private Task<IEnumerable<WorkGroupMemberData>> GetMembersByWorkGroupId(ISqlSettings settings, Guid workGroupId)
        {
            GenericDataFactory<WorkGroupMemberData> genericDataFactory = new GenericDataFactory<WorkGroupMemberData>();

            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "workGroupId", DbType.Guid, workGroupId)
            };
            return genericDataFactory.GetData(settings,
                _providerFactory,
                "[blwt].[GetWorkGroupMember_by_WorkGroupId]",
                () => new WorkGroupMemberData(),
                DataUtil.AssignDataStateManager,
                parameters);
        }

        private Task<IEnumerable<WorkTaskTypeGroupData>> GetTaskTypesByWorkGroupId(ISqlSettings settings, Guid workGroupId)
        {
            GenericDataFactory<WorkTaskTypeGroupData> genericDataFactory = new GenericDataFactory<WorkTaskTypeGroupData>();

            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "workGroupId", DbType.Guid, workGroupId)
            };
            return genericDataFactory.GetData(settings,
                _providerFactory,
                "[blwt].[GetWorkTaskTypeGroup_by_WorkGroupId]",
                () => new WorkTaskTypeGroupData(),
                DataUtil.AssignDataStateManager,
                parameters);
        }

        public async Task<WorkGroupData> Get(ISqlSettings settings, Guid id)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid, id)
            };
            Task<IEnumerable<WorkGroupMemberData>> getMembersByWorkGroupId = GetMembersByWorkGroupId(settings, id);
            Task<IEnumerable<WorkTaskTypeGroupData>> getTaskTypesByWorkGroupId = GetTaskTypesByWorkGroupId(settings, id);
            WorkGroupData data = (await _genericDataFactory.GetData(settings,
                _providerFactory,
                "[blwt].[GetWorkGroup]",
                Create,
                DataUtil.AssignDataStateManager,
                parameters))
                .FirstOrDefault();

            if (data != null)
            {
                data.Members = (await getMembersByWorkGroupId).ToList();
                data.TaskTypes = (await getTaskTypesByWorkGroupId).ToList();
            }
            return data;
        }

        public async Task<IEnumerable<WorkGroupData>> GetByDomainId(ISqlSettings settings, Guid domainId)
        {
            List<WorkGroupData> workGroups = new List<WorkGroupData>();
            List<WorkGroupMemberData> members = new List<WorkGroupMemberData>();
            List<WorkTaskTypeGroupData> taskTypes = new List<WorkTaskTypeGroupData>();
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "domainId", DbType.Guid, domainId)
            };
            DataReaderProcess dataReaderProcess = new DataReaderProcess();
            await dataReaderProcess.Read(settings, _providerFactory, "[blwt].[GetWorkGroup_by_DomainId]", CommandType.StoredProcedure,
                parameters,
                readAction: async (DbDataReader reader) =>
                {
                    workGroups = (await _genericDataFactory.LoadData(reader, Create, DataUtil.AssignDataStateManager)).ToList();
                    if (reader.NextResult())
                    {
                        GenericDataFactory<WorkGroupMemberData> genericDataFactory = new GenericDataFactory<WorkGroupMemberData>();
                        members = (await genericDataFactory.LoadData(reader, () => new WorkGroupMemberData(), DataUtil.AssignDataStateManager)).ToList();
                    }
                    if (reader.NextResult())
                    {
                        GenericDataFactory<WorkTaskTypeGroupData> genericDataFactory = new GenericDataFactory<WorkTaskTypeGroupData>();
                        taskTypes = (await genericDataFactory.LoadData(reader, () => new WorkTaskTypeGroupData(), DataUtil.AssignDataStateManager)).ToList();
                    }
                });
            workGroups = workGroups.GroupJoin<WorkGroupData, WorkGroupMemberData, Guid, WorkGroupData>(members, g => g.WorkGroupId, m => m.WorkGroupId, 
                (g, mbrs) => 
                {
                    g.Members = mbrs.ToList();
                    return g;
                })
                .GroupJoin<WorkGroupData, WorkTaskTypeGroupData, Guid, WorkGroupData>(taskTypes, g => g.WorkGroupId, tt => tt.WorkGroupId,
                (g, tts) =>
                {
                    g.TaskTypes = tts.ToList();
                    return g;
                }
                )
                .ToList();
            return workGroups;
        }
    }
}
