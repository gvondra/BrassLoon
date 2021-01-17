using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Config.Framework
{
    public interface IItemSaver
    {
        Task Create(ISettings settings, IItem item);
        Task Update(ISettings settings, IItem item);
        Task DeleteByCode(ISettings settings, Guid domainId, string code);
    }
}
