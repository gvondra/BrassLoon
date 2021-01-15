using BrassLoon.Config.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Config.Data
{
    public interface IItemDataFactory
    {
        Task<IEnumerable<string>> GetCodes(ISqlSettings settings, Guid domainId);
        Task<ItemData> GetByCode(ISqlSettings settings, Guid domainId, string code);
    }
}
