using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Config.Framework
{
    public interface IItemHistoryFactory
    {
        Task<IEnumerable<IItemHistory>> GetByItemId(ISettings settings, Guid itemId);
    }
}
