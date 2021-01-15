﻿using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Config.Framework
{
    public interface IItemFactory
    {
        Task<IEnumerable<string>> GetCodes(ISettings settings, Guid domainId);
        Task<IItem> GetByCode(ISettings settings, Guid domainId, string code);
    }
}
