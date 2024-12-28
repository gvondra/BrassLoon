using BrassLoon.CommonData;
using BrassLoon.Config.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Config.Data
{
    public interface IItemHistoryDataFactory
    {
        Task<IEnumerable<ItemHistoryData>> GetByItemId(ISettings settings, Guid itemId);
    }
}
