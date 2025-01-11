using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Internal.SqlClient
{
    public class SigningKeyDataFactory : DataFactoryBase<SigningKeyData>, ISigningKeyDataFactory
    {
        public SigningKeyDataFactory(IDbProviderFactory providerFactory)
            : base(providerFactory) { }

        public async Task<SigningKeyData> Get(CommonData.ISettings settings, Guid id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid, id);
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[blt].[GetSigningKey]",
                Create,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }))
                .FirstOrDefault();
        }

        public async Task<IEnumerable<SigningKeyData>> GetByDomainId(CommonData.ISettings settings, Guid domainId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "domainId", DbType.Guid, domainId);
            return await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[blt].[GetSigningKey_by_DomainId]",
                Create,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter })
                ;
        }
    }
}
