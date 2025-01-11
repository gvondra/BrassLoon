using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Internal.SqlClient
{
    public class EmailAddressDataFactory : DataFactoryBase<EmailAddressData>, IEmailAddressDataFactory
    {
        public EmailAddressDataFactory(IDbProviderFactory providerFactory)
            : base(providerFactory) { }

        public async Task<EmailAddressData> Get(CommonData.ISettings settings, Guid id)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid, id)
            };
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[blt].[GetEmailAddress]",
                Create,
                DataUtil.AssignDataStateManager,
                parameters))
                .FirstOrDefault();
        }

        public async Task<EmailAddressData> GetByAddressHash(CommonData.ISettings settings, byte[] hash)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "addressHash", DbType.Binary, hash)
            };
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[blt].[GetEmailAddress_by_AddressHash]",
                Create,
                DataUtil.AssignDataStateManager,
                parameters))
                .FirstOrDefault();
        }
    }
}
