using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal
{
    public class WorkTaskTypeDataFactory : DataFactoryBase<WorkTaskTypeData>, IWorkTaskTypeDataFactory
    {
        public WorkTaskTypeDataFactory(IDbProviderFactory providerFactory) : base(providerFactory) { }

        public async Task<WorkTaskTypeData> Get(ISqlSettings settings, Guid id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(ProviderFactory, "id", DbType.Guid, id);
            return (await GenericDataFactory.GetData(settings,
                ProviderFactory,
                "[blwt].[GetWorkTaskType]",
                Create,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }))
                .FirstOrDefault();
        }

        public async Task<IEnumerable<WorkTaskTypeData>> GetByDomainId(ISqlSettings settings, Guid domainId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(ProviderFactory, "domainId", DbType.Guid, domainId);
            return await GenericDataFactory.GetData(settings,
                ProviderFactory,
                "[blwt].[GetWorkTaskType_by_DomainId]",
                Create,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter })
                ;
        }

        public async Task<WorkTaskTypeData> GetByDomainIdCode(ISqlSettings settings, Guid domainId, string code)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(ProviderFactory, "domainId", DbType.Guid, domainId),
                DataUtil.CreateParameter(ProviderFactory, "code", DbType.String, code)
            };
            return (await GenericDataFactory.GetData(settings,
                ProviderFactory,
                "[blwt].[GetWorkTaskType_by_DomainId_Code]",
                Create,
                DataUtil.AssignDataStateManager,
                parameters))
                .FirstOrDefault();
        }

        public async Task<IEnumerable<WorkTaskTypeData>> GetByWorkGroupId(ISqlSettings settings, Guid workGroupId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(ProviderFactory, "workGroupId", DbType.Guid, workGroupId);
            return await GenericDataFactory.GetData(settings,
                ProviderFactory,
                "[blwt].[GetWorkTaskType_by_WorkGroupId]",
                Create,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter })
                ;
        }
    }
}
