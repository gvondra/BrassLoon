﻿using BrassLoon.Config.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Config.Data
{
    public interface ILookupDataFactory
    {
        Task<IEnumerable<string>> GetCodes(ISqlSettings settings, Guid domainId);
        Task<LookupData> GetByCode(ISqlSettings settings, Guid domainId, string code);
    }
}
