using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public class UserInvitationDataFactory : IUserInvitationDataFactory
    {
        private ISqlDbProviderFactory _providerFactory;
        private GenericDataFactory<UserInvitationData> _genericDataFactory;

        public UserInvitationDataFactory(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
            _genericDataFactory = new GenericDataFactory<UserInvitationData>();
        }

        public async Task<UserInvitationData> Get(ISqlSettings settings, Guid id)
        {
            IDataParameter[] parameters = 
            {
                DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid, id)
            };
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[bla].[GetUserInvitation]",
                () => new UserInvitationData(),
                DataUtil.AssignDataStateManager,
                parameters
                )).FirstOrDefault();
        }

        public async Task<IEnumerable<UserInvitationData>> GetByAccountId(ISqlSettings settings, Guid accountId)
        {
            IDataParameter[] parameters =
            {
                DataUtil.CreateParameter(_providerFactory, "accountGuid", DbType.Guid, accountId)
            };
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[bla].[GetUserInvitationByAccountGuid]",
                () => new UserInvitationData(),
                DataUtil.AssignDataStateManager,
                parameters
                ));
        }
    }
}
