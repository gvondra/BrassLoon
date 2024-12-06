using BrassLoon.Config.Data.Models;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Config.Data
{
    public interface ILookupDataSaver
    {
        Task Create(ISaveSettings saveSettings, LookupData lookupData);
        Task Update(ISaveSettings saveSettings, LookupData lookupData);
        Task DeleteByCode(ISaveSettings saveSettings, Guid domainId, string code);
    }
}
