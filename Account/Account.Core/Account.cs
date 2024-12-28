using BrassLoon.Account.Data;
using BrassLoon.Account.Data.Models;
using BrassLoon.Account.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class Account : IAccount
    {
        private readonly AccountData _data;
        private readonly IAccountDataSaver _dataSaver;

        public Account(
            AccountData accountData,
            IAccountDataSaver dataSaver)
        {
            _data = accountData;
            _dataSaver = dataSaver;
        }

        public Guid AccountId => _data.AccountGuid;

        public string Name { get => _data.Name; set => _data.Name = value; }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        public bool Locked => _data.Locked;

        public async Task Create(CommonCore.ISaveSettings saveSettings, Guid userId) => await _dataSaver.Create(saveSettings, userId, _data);

        public Task<IEnumerable<IDomain>> GetDomains(ISettings settings) => throw new NotImplementedException();

        public async Task Update(CommonCore.ISaveSettings saveSettings) => await _dataSaver.Update(saveSettings, _data);
    }
}
