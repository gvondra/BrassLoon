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
    public class WorkTaskStatusDataFactory : DataFactoryBase<WorkTaskStatusData>, IWorkTaskStatusDataFactory
    {
        public WorkTaskStatusDataFactory(IDbProviderFactory providerFactory) : base(providerFactory) { }

        public async Task<WorkTaskStatusData> Get(ISqlSettings settings, Guid id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid, id);
            return (await _genericDataFactory.GetData(settings,
                _providerFactory,
                "[blwt].[GetWorkTaskStatus]",
                Create,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }))
                .FirstOrDefault();
        }

        public async Task<IEnumerable<WorkTaskStatusData>> GetByWorkTaskType(ISqlSettings settings, Guid workTaskTypeId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "workTaskTypeId", DbType.Guid, workTaskTypeId);
            return await _genericDataFactory.GetData(settings,
                _providerFactory,
                "[blwt].[GetWorkTaskStatus_by_WorkTaskTypeId]",
                Create,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter })
                ;
        }
    }
}
