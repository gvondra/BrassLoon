﻿using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public class UserDataFactory : IUserDataFactory
    {
        private ISqlDbProviderFactory _providerFactory;
        private GenericDataFactory<UserData> _genericDataFactory;

        public UserDataFactory(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
            _genericDataFactory = new GenericDataFactory<UserData>();
        }

        public async Task<UserData> Get(ISqlSettings settings, Guid id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "guid", DbType.Guid, id);
            return (await _genericDataFactory.GetData(
                settings, 
                _providerFactory, 
                "[bla].[GetUser]", 
                () => new UserData(), 
                DataUtil.AssignDataStateManager, 
                new List<IDataParameter> { parameter }
                )).FirstOrDefault();
        }

        public async Task<UserData> GetByReferenceId(ISqlSettings settings, string referenceId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "referenceId", DbType.AnsiString, referenceId);
            return (await _genericDataFactory.GetData(
                settings, 
                _providerFactory, 
                "[bla].[GetUserByReferenceId]", 
                () => new UserData(),
                DataUtil.AssignDataStateManager, 
                new List<IDataParameter> { parameter }
                )).FirstOrDefault();
        }

        public async Task<IEnumerable<UserData>> GetByEmailAddress(ISqlSettings settings, string emailAddress)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "address", DbType.String, emailAddress);
            return await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[bla].[GetUserByEmailAddress]",
                () => new UserData(),
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }
                );
        }
    }
}
