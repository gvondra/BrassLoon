﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Framework
{
    public interface IClientFactory
    {
        IClient Create(Guid domainId, string secret);
        Task<IClient> Get(ISettings settings, Guid domainId, Guid id);
        Task<IEnumerable<IClient>> GetByDomainId(ISettings settings, Guid domainId);
    }
}
