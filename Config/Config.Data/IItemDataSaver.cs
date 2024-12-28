using BrassLoon.CommonData;
using BrassLoon.Config.Data.Models;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Config.Data
{
    public interface IItemDataSaver
    {
        Task Create(ISaveSettings saveSettings, ItemData itemData);
        Task Update(ISaveSettings saveSettings, ItemData itemData);
        Task DeleteByCode(ISaveSettings saveSettings, Guid domainId, string code);
    }
}
