using BrassLoon.Config.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Config.Data
{
    public interface IItemHistoryDataFactory
    {
        Task<IEnumerable<ItemHistoryData>> GetByItemId(ISqlSettings settings, Guid itemId);
    }
}
