using BrassLoon.Config.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Config.Data
{
    public interface ILookupDataFactory
    {
        Task<IEnumerable<string>> GetCodes(ISettings settings, Guid domainId);
        Task<LookupData> GetByCode(ISettings settings, Guid domainId, string code);
    }
}
