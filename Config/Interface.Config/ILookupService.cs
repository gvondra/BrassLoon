using BrassLoon.Interface.Config.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Config
{
    public interface ILookupService
    {
        Task<List<string>> GetCodes(ISettings settings, Guid domainId);
        Task<Lookup> GetByCode(ISettings settings, Guid domainId, string code);
        Task<Dictionary<string, string>> GetDataByCode(ISettings settings, Guid domainId, string code);
        Task<List<LookupHistory>> GetHistoryByCode(ISettings settings, Guid domainId, string code);
        Task<Lookup> Save(ISettings settings, Guid domainId, string code, object data);
        Task Delete(ISettings settings, Guid domainId, string code);
    }
}
