﻿using BrassLoon.Account.Data;
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

        public async Task Create(Framework.ISaveSettings settings, Guid userId) => await _dataSaver.Create(new DataSaveSettings(settings), userId, _data);

        public Task<IEnumerable<IDomain>> GetDomains(Framework.ISettings settings) => throw new NotImplementedException();

        public async Task Update(Framework.ISaveSettings settings) => await _dataSaver.Update(new DataSaveSettings(settings), _data);
    }
}
