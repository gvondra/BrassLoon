using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal
{
    public class WorkGroupDataFactory : DataFactoryBase<WorkGroupData>, IWorkGroupDataFactory
    {
        public WorkGroupDataFactory(IDbProviderFactory providerFactory) : base(providerFactory) { }

        public async Task<WorkGroupData> Get(ISqlSettings settings, Guid id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid, id);
            return (await _genericDataFactory.GetData(settings,
                _providerFactory,
                "[blwt].[GetWorkGroup]",
                Create,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }))
                .FirstOrDefault();
        }

        public async Task<IEnumerable<WorkGroupData>> GetByDomainId(ISqlSettings settings, Guid domainId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "domainId", DbType.Guid, domainId);
            return await _genericDataFactory.GetData(settings,
                _providerFactory,
                "[blwt].[GetWorkGroup_by_DomainId]",
                Create,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter })
                ;
        }
    }
}
