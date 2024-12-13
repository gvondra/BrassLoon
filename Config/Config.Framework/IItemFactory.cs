using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Config.Framework
{
    public interface IItemFactory
    {
        IItem Create(Guid domainId, string code);
        Task<IEnumerable<string>> GetCodes(ISettings settings, Guid domainId);
        Task<IItem> GetByCode(ISettings settings, Guid domainId, string code);
    }
}
