using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal.SqlClient
{
    public class WorkTaskStatusDataFactory : DataFactoryBase<WorkTaskStatusData>, IWorkTaskStatusDataFactory
    {
        public WorkTaskStatusDataFactory(IDbProviderFactory providerFactory)
            : base(providerFactory) { }

        public async Task<WorkTaskStatusData> Get(ISqlSettings settings, Guid id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(ProviderFactory, "id", DbType.Guid, id);
            return (await GenericDataFactory.GetData(
                settings,
                ProviderFactory,
                "[blwt].[GetWorkTaskStatus]",
                Create,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }))
                .FirstOrDefault();
        }

        public async Task<IEnumerable<WorkTaskStatusData>> GetByDomainId(ISqlSettings settings, Guid domainId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(ProviderFactory, "domainId", DbType.Guid, domainId);
            return await GenericDataFactory.GetData(
                settings,
                ProviderFactory,
                "[blwt].[GetWorkTaskStatus_by_DomainId]",
                Create,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter })
                ;
        }

        public async Task<IEnumerable<WorkTaskStatusData>> GetByWorkTaskType(ISqlSettings settings, Guid workTaskTypeId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(ProviderFactory, "workTaskTypeId", DbType.Guid, workTaskTypeId);
            return await GenericDataFactory.GetData(
                settings,
                ProviderFactory,
                "[blwt].[GetWorkTaskStatus_by_WorkTaskTypeId]",
                Create,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter })
                ;
        }
    }
}
