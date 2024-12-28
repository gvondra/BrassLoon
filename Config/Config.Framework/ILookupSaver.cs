using BrassLoon.CommonCore;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Config.Framework
{
    public interface ILookupSaver
    {
        Task Create(ISettings settings, ILookup lookup);
        Task Update(ISettings settings, ILookup lookup);
        Task DeleteByCode(ISettings settings, Guid domainId, string code);
    }
}
