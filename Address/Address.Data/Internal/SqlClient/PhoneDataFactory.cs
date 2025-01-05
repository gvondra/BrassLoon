using BrassLoon.Address.Data.Models;
using BrassLoon.DataClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Address.Data.Internal.SqlClient
{
    public class PhoneDataFactory : DataFactoryBase<PhoneData>, IPhoneDataFactory
    {
        public PhoneDataFactory(IDbProviderFactory providerFactory)
            : base(providerFactory)
        { }

        public async Task<PhoneData> Get(ISqlSettings settings, Guid id)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(ProviderFactory, "id", DbType.Guid, id)
            };
            return (await GenericDataFactory.GetData(settings,
                ProviderFactory,
                "[blad].[GetPhone]",
                Create,
                DataUtil.AssignDataStateManager,
                parameters))
                .FirstOrDefault();
        }

        public async Task<IEnumerable<PhoneData>> GetByHash(ISqlSettings settings, Guid domainId, byte[] hash)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(ProviderFactory, "domainId", DbType.Guid, domainId),
                DataUtil.CreateParameter(ProviderFactory, "hash", DbType.Binary, hash)
            };
            return await GenericDataFactory.GetData(settings,
                ProviderFactory,
                "[blad].[GetPhone_by_Hash]",
                Create,
                DataUtil.AssignDataStateManager,
                parameters);
        }
    }
}
