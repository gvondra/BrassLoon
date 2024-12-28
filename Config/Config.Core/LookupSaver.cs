using BrassLoon.CommonCore;
using BrassLoon.Config.Data;
using BrassLoon.Config.Framework;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Config.Core
{
    public class LookupSaver : ILookupSaver
    {
        private readonly ILookupDataSaver _dataSaver;

        public LookupSaver(ILookupDataSaver dataSaver)
        {
            _dataSaver = dataSaver;
        }

        public async Task Create(ISettings settings, ILookup lookup)
            => await Saver.Save(new SaveSettings(settings), lookup.Create);

        public async Task DeleteByCode(ISettings settings, Guid domainId, string code)
            => await Saver.Save(new SaveSettings(settings), ss => _dataSaver.DeleteByCode(ss, domainId, code));

        public async Task Update(ISettings settings, ILookup lookup)
            => await Saver.Save(new SaveSettings(settings), lookup.Update);
    }
}
