using BrassLoon.Address.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Address.Data.Internal
{
    public class AddressDataFactory : DataFactoryBase<AddressData>, IAddressDataFactory
    {
        public AddressDataFactory(IDbProviderFactory providerFactory) : base(providerFactory)
        { }

        public async Task<AddressData> Get(ISqlSettings settings, Guid id)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid, id)
            };
            return (await _genericDataFactory.GetData(settings,
                _providerFactory,
                "[blad].[GetAddress]",
                Create,
                DataUtil.AssignDataStateManager,
                parameters))
                .FirstOrDefault();
        }

        public async Task<IEnumerable<AddressData>> GetByHash(ISqlSettings settings, byte[] hash)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "hash", DbType.Binary, hash)
            };
            return await _genericDataFactory.GetData(settings,
                _providerFactory,
                "[blad].[GetAddress_by_Hash]",
                Create,
                DataUtil.AssignDataStateManager,
                parameters);
        }
    }
}
