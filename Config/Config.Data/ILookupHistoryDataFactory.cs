﻿using BrassLoon.Config.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Config.Data
{
    public interface ILookupHistoryDataFactory
    {
        Task<IEnumerable<LookupHistoryData>> GetByLookupId(ISettings settings, Guid lookupId);
    }
}
