using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal.SqlClient
{
    public class WorkTaskCommentDataFactory : DataFactoryBase<CommentData>, IWorkTaskCommentDataFactory
    {
        public WorkTaskCommentDataFactory(IDbProviderFactory providerFactory)
            : base(providerFactory) { }

        public async Task<IEnumerable<CommentData>> GetByWorkTaskId(ISqlSettings settings, Guid workTaskId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(ProviderFactory, "workTaskId", DbType.Guid, workTaskId);
            return await GenericDataFactory.GetData(
                settings,
                ProviderFactory,
                "[blwt].[GetWorkTaskComment_by_WorkTaskId]",
                Create,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter })
                ;
        }
    }
}
