using BrassLoon.Address.Data.Models;
using BrassLoon.DataClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Address.Data.Internal
{
    public class EmailAddressDataFactory : DataFactoryBase<EmailAddressData>, IEmailAddressDataFactory
    {
        public EmailAddressDataFactory(IDbProviderFactory providerFactory)
            : base(providerFactory)
        { }

        public async Task<EmailAddressData> Get(ISqlSettings settings, Guid id)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(ProviderFactory, "id", DbType.Guid, id)
            };
            return (await GenericDataFactory.GetData(settings,
                ProviderFactory,
                "[blad].[GetEmailAddress]",
                Create,
                DataUtil.AssignDataStateManager,
                parameters))
                .FirstOrDefault();
        }

        public async Task<IEnumerable<EmailAddressData>> GetByHash(ISqlSettings settings, Guid domainId, byte[] hash)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(ProviderFactory, "domainId", DbType.Guid, domainId),
                DataUtil.CreateParameter(ProviderFactory, "hash", DbType.Binary, hash)
            };
            return await GenericDataFactory.GetData(settings,
                ProviderFactory,
                "[blad].[GetEmailAddress_by_Hash]",
                Create,
                DataUtil.AssignDataStateManager,
                parameters);
        }
    }
}
