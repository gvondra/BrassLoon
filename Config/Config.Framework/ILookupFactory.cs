using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Config.Framework
{
    public interface ILookupFactory
    {
        ILookup Create(Guid domainId, string code);
        Task<IEnumerable<string>> GetCodes(ISettings settings, Guid domainId);
        Task<ILookup> GetByCode(ISettings settings, Guid domainId, string code);
    }
}
