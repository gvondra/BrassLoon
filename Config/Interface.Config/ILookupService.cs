using BrassLoon.Interface.Config.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Config
{
    public interface ILookupService
    {
        Task<List<string>> GetCodes(ISettings settings, Guid domainId);
        Task<Lookup> GetByCode(ISettings settings, Guid domainId, string code);
        Task<Dictionary<string, string>> GetDataByCode(ISettings settings, Guid domainId, string code);
        Task<List<LookupHistory>> GetHistoryByCode(ISettings settings, Guid domainId, string code);
        [Obsolete("the data parameter must of type Dictionary<string, string>")]
        Task<Lookup> Save(ISettings settings, Guid domainId, string code, object data);
        Task<Lookup> Save(ISettings settings, Guid domainId, string code, Dictionary<string, string> data);
        Task Delete(ISettings settings, Guid domainId, string code);
    }
}
