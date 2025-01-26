using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal.SqlClient
{
    public class WorkGroupDataFactory : DataFactoryBase<WorkGroupData>, IWorkGroupDataFactory
    {
        public WorkGroupDataFactory(IDbProviderFactory providerFactory)
            : base(providerFactory)
        { }

        public async Task<WorkGroupData> Get(CommonData.ISettings settings, Guid id)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(ProviderFactory, "id", DbType.Guid, id)
            };
            Task<IEnumerable<WorkGroupMemberData>> getMembersByWorkGroupId = GetMembersByWorkGroupId(settings, id);
            Task<IEnumerable<WorkTaskTypeGroupData>> getTaskTypesByWorkGroupId = GetTaskTypesByWorkGroupId(settings, id);
            WorkGroupData data = (await GenericDataFactory.GetData(
                settings,
                ProviderFactory,
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

        public Task<IEnumerable<WorkGroupData>> GetByDomainId(CommonData.ISettings settings, Guid domainId)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(ProviderFactory, "domainId", DbType.Guid, domainId)
            };
            return InnerGetData(settings, "[blwt].[GetWorkGroup_by_DomainId]", parameters);
        }

        public Task<IEnumerable<WorkGroupData>> GetByMemberUserId(CommonData.ISettings settings, Guid domainId, string userId)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(ProviderFactory, "domainId", DbType.Guid, domainId),
                DataUtil.CreateParameter(ProviderFactory, "userId", DbType.AnsiString, userId)
            };
            return InnerGetData(settings, "[blwt].[GetWorkGroup_by_MemberUserId]", parameters);
        }

        private Task<IEnumerable<WorkGroupMemberData>> GetMembersByWorkGroupId(CommonData.ISettings settings, Guid workGroupId)
        {
            GenericDataFactory<WorkGroupMemberData> genericDataFactory = new GenericDataFactory<WorkGroupMemberData>();

            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(ProviderFactory, "workGroupId", DbType.Guid, workGroupId)
            };
            return genericDataFactory.GetData(
                settings,
                ProviderFactory,
                "[blwt].[GetWorkGroupMember_by_WorkGroupId]",
                () => new WorkGroupMemberData(),
                DataUtil.AssignDataStateManager,
                parameters);
        }

        private Task<IEnumerable<WorkTaskTypeGroupData>> GetTaskTypesByWorkGroupId(CommonData.ISettings settings, Guid workGroupId)
        {
            GenericDataFactory<WorkTaskTypeGroupData> genericDataFactory = new GenericDataFactory<WorkTaskTypeGroupData>();

            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(ProviderFactory, "workGroupId", DbType.Guid, workGroupId)
            };
            return genericDataFactory.GetData(
                settings,
                ProviderFactory,
                "[blwt].[GetWorkTaskTypeGroup_by_WorkGroupId]",
                () => new WorkTaskTypeGroupData(),
                DataUtil.AssignDataStateManager,
                parameters);
        }

        private async Task<IEnumerable<WorkGroupData>> InnerGetData(CommonData.ISettings settings, string procedureName, IDataParameter[] parameters)
        {
            List<WorkGroupData> workGroups = new List<WorkGroupData>();
            List<WorkGroupMemberData> members = new List<WorkGroupMemberData>();
            List<WorkTaskTypeGroupData> taskTypes = new List<WorkTaskTypeGroupData>();
            DataReaderProcess dataReaderProcess = new DataReaderProcess();
            await dataReaderProcess.Read(
                settings,
                ProviderFactory,
                procedureName,
                CommandType.StoredProcedure,
                parameters,
                readAction: async (reader) =>
                {
                    workGroups = (await GenericDataFactory.LoadData(reader, Create, DataUtil.AssignDataStateManager)).ToList();
                    if (await reader.NextResultAsync())
                    {
                        GenericDataFactory<WorkGroupMemberData> genericDataFactory = new GenericDataFactory<WorkGroupMemberData>();
                        members = (await genericDataFactory.LoadData(reader, () => new WorkGroupMemberData(), DataUtil.AssignDataStateManager)).ToList();
                    }
                    if (await reader.NextResultAsync())
                    {
                        GenericDataFactory<WorkTaskTypeGroupData> genericDataFactory = new GenericDataFactory<WorkTaskTypeGroupData>();
                        taskTypes = (await genericDataFactory.LoadData(reader, () => new WorkTaskTypeGroupData(), DataUtil.AssignDataStateManager)).ToList();
                    }
                });
            workGroups = workGroups.GroupJoin(
                members,
                g => g.WorkGroupId,
                m => m.WorkGroupId,
                (g, mbrs) =>
                {
                    g.Members = mbrs.ToList();
                    return g;
                })
                .GroupJoin(
                taskTypes,
                g => g.WorkGroupId,
                tt => tt.WorkGroupId,
                (g, tts) =>
                {
                    g.TaskTypes = tts.ToList();
                    return g;
                })
                .ToList();
            return workGroups;
        }
    }
}
