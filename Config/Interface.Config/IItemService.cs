using BrassLoon.Interface.Config.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Config
{
    public interface IItemService
    {
        Task<List<string>> GetCodes(ISettings settings, Guid domainId);
        Task<Item> GetByCode(ISettings settings, Guid domainId, string code);
        Task<object> GetDataByCode(ISettings settings, Guid domainId, string code);
        Task<List<ItemHistory>> GetHistoryByCode(ISettings settings, Guid domainId, string code);
        Task<Item> Save(ISettings settings, Guid domainId, string code, object data);
        Task Delete(ISettings settings, Guid domainId, string code);
    }
}
