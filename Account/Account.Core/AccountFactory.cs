using BrassLoon.Account.Data;
using BrassLoon.Account.Data.Models;
using BrassLoon.Account.Framework;
using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class AccountFactory : IAccountFactory
    {
        private IAccountDataFactory _dataFactory;
        private IAccountDataSaver _dataSaver;
        private SettingsFactory _settingsFactory;

        public AccountFactory(IAccountDataFactory dataFactory,
            IAccountDataSaver dataSaver,
            SettingsFactory settingsFactory)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
            _settingsFactory = settingsFactory;
        }

        public IAccount Create()
        {
            return new Account(new AccountData(), _dataSaver);
        }

        public async Task<IAccount> Get(ISettings settings, Guid id)
        {
            Account result = null;
            AccountData data = await _dataFactory.Get(_settingsFactory.CreateData(settings), id);
            if (data != null)
                result = new Account(data, _dataSaver);
            return result;
        }

        public async Task<IEnumerable<Guid>> GetAccountIdsByUserId(ISettings settings, Guid userId)
        {
            return await _dataFactory.GetAccountIdsByUserId(_settingsFactory.CreateData(settings), userId);
        }

        public async Task<IEnumerable<IAccount>> GetByUserId(ISettings settings, Guid userId)
        {
            return (await _dataFactory.GetByUserId(_settingsFactory.CreateData(settings), userId))
                .Select<AccountData, IAccount>(data => new Account(data, _dataSaver));
        }
    }
}
