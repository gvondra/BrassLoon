using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal
{
    public class WorkTaskTypeDataFactory : DataFactoryBase<WorkTaskTypeData>, IWorkTaskTypeDataFactory
    {
        public WorkTaskTypeDataFactory(IDbProviderFactory providerFactory) : base(providerFactory) { }

        public async Task<WorkTaskTypeData> Get(ISqlSettings settings, Guid id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid, id);
            return (await _genericDataFactory.GetData(settings,
                _providerFactory,
                "[blwt].[GetWorkTaskType]",
                Create,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }))
                .FirstOrDefault();
        }

        public async Task<IEnumerable<WorkTaskTypeData>> GetByDomainId(ISqlSettings settings, Guid domainId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "domainId", DbType.Guid, domainId);
            return await _genericDataFactory.GetData(settings,
                _providerFactory,
                "[blwt].[GetWorkTaskType_by_DomainId]",
                Create,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter })
                ;
        }

        public async Task<IEnumerable<WorkTaskTypeData>> GetByWorkGroupId(ISqlSettings settings, Guid workGroupId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "workGroupId", DbType.Guid, workGroupId);
            return await _genericDataFactory.GetData(settings,
                _providerFactory,
                "[blwt].[GetWorkTaskType_by_WorkGroupId]",
                Create,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter })
                ;
        }
    }
}
