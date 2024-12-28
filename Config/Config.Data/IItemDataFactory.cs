using BrassLoon.CommonData;
using BrassLoon.Config.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Config.Data
{
    public interface IItemDataFactory
    {
        Task<IEnumerable<string>> GetCodes(ISettings settings, Guid domainId);
        Task<ItemData> GetByCode(ISettings settings, Guid domainId, string code);
    }
}
