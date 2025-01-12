using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Internal.SqlClient
{
    public class UserDataFactory : DataFactoryBase<UserData>, IUserDataFactory
    {
        private readonly IGenericDataFactory<RoleData> _roleDataFactory;

        public UserDataFactory(
            IDbProviderFactory providerFactory,
            IGenericDataFactory<RoleData> roleDataFactory)
            : base(providerFactory)
        {
            _roleDataFactory = roleDataFactory;
        }

        public async Task<UserData> Get(CommonData.ISettings settings, Guid id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid, id);
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[blt].[GetUser]",
                Create,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }))
                .FirstOrDefault();
        }

        public Task<IEnumerable<UserData>> GetByDomainId(CommonData.ISettings settings, Guid domainId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "domainId", DbType.Guid, domainId);
            return _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[blt].[GetUser_by_DomainId]",
                Create,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter });
        }

        public async Task<UserData> GetByEmailAddressHash(CommonData.ISettings settings, Guid domainId, byte[] hash)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "domainId", DbType.Guid, domainId),
                DataUtil.CreateParameter(_providerFactory, "addressHash", DbType.Binary, hash)
            };
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[blt].[GetUser_by_EmailAddressHash]",
                Create,
                DataUtil.AssignDataStateManager,
                parameters))
                .FirstOrDefault();
        }

        public async Task<UserData> GetByReferenceId(CommonData.ISettings settings, Guid domainId, string referenceId)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "domainId", DbType.Guid, domainId),
                DataUtil.CreateParameter(_providerFactory, "referenceId", DbType.AnsiString, referenceId)
            };
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[blt].[GetUser_by_ReferenceId]",
                Create,
                DataUtil.AssignDataStateManager,
                parameters))
                .FirstOrDefault();
        }

        public async Task<IEnumerable<RoleData>> GetRoles(CommonData.ISettings settings, UserData userData)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "userId", DbType.Guid, userData.UserId)
            };
            return await _roleDataFactory.GetData(
                settings,
                _providerFactory,
                "[blt].[GetRole_by_UserId]",
                () => new RoleData(),
                DataUtil.AssignDataStateManager,
                parameters)
                ;
        }
    }
}
