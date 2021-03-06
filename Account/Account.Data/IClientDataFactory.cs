﻿using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IClientDataFactory
    {
        Task<ClientData> Get(ISqlSettings settings, Guid id);
        Task<IEnumerable<ClientData>> GetByAccountId(ISqlSettings settings, Guid accountId);
    }
}
