﻿using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Framework
{
    public interface IRoleFactory
    {
        IRole Create(Guid domainId, string policyName);
        Task<IEnumerable<IRole>> GetByDomainId(ISettings settings, Guid domainId);
        Task<IRole> Get(ISettings settings, Guid id);
    }
}