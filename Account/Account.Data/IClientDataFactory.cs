﻿using BrassLoon.Account.Data.Models;
using BrassLoon.CommonData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IClientDataFactory
    {
        Task<ClientData> Get(ISettings settings, Guid id);
        Task<IEnumerable<ClientData>> GetByAccountId(ISettings settings, Guid accountId);
    }
}
