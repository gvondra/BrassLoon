﻿using BrassLoon.Account.Data;
using BrassLoon.Account.Data.Models;
using BrassLoon.Account.Framework;
using BrassLoon.CommonCore;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class EmailAddress : IEmailAddress
    {
        private readonly EmailAddressData _emailAddressData;
        private readonly IEmailAddressDataSaver _dataSaver;

        public EmailAddress(
            EmailAddressData emailAddressData,
            IEmailAddressDataSaver dataSaver)
        {
            _emailAddressData = emailAddressData;
            _dataSaver = dataSaver;
        }

        public Guid EmailAddressId => _emailAddressData.EmailAddressGuid;

        public string Address => _emailAddressData.Address;

        public DateTime CreateTimestamp => _emailAddressData.CreateTimestamp;

        public async Task Create(ITransactionHandler transactionHandler) => await _dataSaver.Create(transactionHandler, _emailAddressData);
    }
}
