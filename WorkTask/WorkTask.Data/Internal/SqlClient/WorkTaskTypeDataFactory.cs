using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal.SqlClient
{
    public class WorkTaskTypeDataFactory : DataFactoryBase<WorkTaskTypeData>, IWorkTaskTypeDataFactory
    {
        private readonly IGenericDataFactory<WorkTaskStatusData> _statusDataFactory;

        public WorkTaskTypeDataFactory(IDbProviderFactory providerFactory, IGenericDataFactory<WorkTaskStatusData> statusDataFactory)
            : base(providerFactory)
        {
            _statusDataFactory = statusDataFactory;
        }

        public async Task<WorkTaskTypeData> Get(CommonData.ISettings settings, Guid id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(ProviderFactory, "id", DbType.Guid, id);
            DataReaderProcess dataReaderProcess = new DataReaderProcess();
            return (await dataReaderProcess.Read(
                settings,
                ProviderFactory,
                "[blwt].[GetWorkTaskType]",
                CommandType.StoredProcedure,
                new List<IDataParameter> { parameter },
                Read))
                .FirstOrDefault();
        }

        public async Task<IEnumerable<WorkTaskTypeData>> GetByDomainId(CommonData.ISettings settings, Guid domainId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(ProviderFactory, "domainId", DbType.Guid, domainId);
            DataReaderProcess dataReaderProcess = new DataReaderProcess();
            return await dataReaderProcess.Read(
                settings,
                ProviderFactory,
                "[blwt].[GetWorkTaskType_by_DomainId]",
                CommandType.StoredProcedure,
                new List<IDataParameter> { parameter },
                Read);
        }

        public async Task<WorkTaskTypeData> GetByDomainIdCode(CommonData.ISettings settings, Guid domainId, string code)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(ProviderFactory, "domainId", DbType.Guid, domainId),
                DataUtil.CreateParameter(ProviderFactory, "code", DbType.String, code)
            };
            DataReaderProcess dataReaderProcess = new DataReaderProcess();
            return (await dataReaderProcess.Read(
                settings,
                ProviderFactory,
                "[blwt].[GetWorkTaskType_by_DomainId_Code]",
                CommandType.StoredProcedure,
                parameters,
                Read))
                .FirstOrDefault();
        }

        public async Task<IEnumerable<WorkTaskTypeData>> GetByWorkGroupId(CommonData.ISettings settings, Guid workGroupId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(ProviderFactory, "workGroupId", DbType.Guid, workGroupId);
            DataReaderProcess dataReaderProcess = new DataReaderProcess();
            return await dataReaderProcess.Read(
                settings,
                ProviderFactory,
                "[blwt].[GetWorkTaskType_by_WorkGroupId]",
                CommandType.StoredProcedure,
                new List<IDataParameter> { parameter },
                Read);
        }

        private async Task<List<WorkTaskTypeData>> Read(DbDataReader reader)
        {
            IEnumerable<WorkTaskTypeData> types = await GenericDataFactory.LoadData(reader, Create, DataUtil.AssignDataStateManager);
            IEnumerable<WorkTaskStatusData> statuses = await reader.NextResultAsync()
                    ? await _statusDataFactory.LoadData(reader, () => new WorkTaskStatusData(), DataUtil.AssignDataStateManager)
                    : Enumerable.Empty<WorkTaskStatusData>();
            return types
                .GroupJoin(
                statuses,
                wtt => wtt.WorkTaskTypeId,
                wts => wts.WorkTaskTypeId,
                (wtt, wts) =>
                {
                    wtt.Statuses = wts.ToList();
                    return wtt;
                })
                .ToList();
        }
    }
}
